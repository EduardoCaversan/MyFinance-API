using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MyFinance.Domain.Events;
using MyFinance.Domain.Utils;
using Newtonsoft.Json.Linq;

namespace MyFinance.Domain.Commands
{
    public class CommandsHandler
    {
        private readonly ListenersHandler _listenersHandler;

        public RequestData CurrentRequest { get; set; }
        public CommandsDbContext DbContext { get; private set; }

        public CommandsHandler(CommandsDbContext context, ListenersHandler listenersHandler)
        {
            DbContext = context;
            _listenersHandler = listenersHandler;
        }

        public async Task<CommandResult> Handle(ICommand command, bool calcTime = false)
        {
            var sw = calcTime ? Stopwatch.StartNew() : null;
            var result = await _Handle(command);
            return _GetResult(result, sw);
        }

        private async Task<CommandResult> _Handle(ICommand command)
        {
            var result = await command.GetErrorAsync(this);
            if (result != null && result.ErrorCode != ErrorCode.None)
                return result;

            var hasPermission = await command.HasPermissionAsync(this);
            if (!hasPermission)
                return new CommandResult(ErrorCode.Unauthorized, "Você não está autorizado a executar esse comando.");

            result = await command.ExecuteAsync(this);
            if (result != null && result.ErrorCode != ErrorCode.None)
                return result;

            await _listenersHandler.HandleEvent(this, command, result);
            return result;
        }

        private CommandResult _GetResult(CommandResult result, Stopwatch stopWatch)
        {
            if (stopWatch == null)
                return result;
            if (stopWatch.IsRunning)
                stopWatch.Stop();
            result.Time = stopWatch.Elapsed.ToString("mm\\:ss\\.fffffff");
            return result;
        }

        public async Task<CommandResult[]> Handle(ICommand[] commands, bool calcTime = false)
        {
            if (commands == null)
                return new[] { new CommandResult(ErrorCode.InvalidParameters, "Commands parameter cannot be null.") };

            var lst = new List<CommandResult>();
            foreach (var c in commands)
            {
                var rs = c == null ?
                    new CommandResult(ErrorCode.InvalidParameters, "Command not found.") :
                    await Handle(c, calcTime);
                lst.Add(rs);
            }
            return lst.ToArray();
        }

        public async Task<CommandResult[]> Handle(CommandData[] commands, bool calcTime = false)
        {
            if (commands == null)
                return new[] { new CommandResult(ErrorCode.InvalidParameters, "Commands parameter cannot be null.") };

            var lst = new List<ICommand>();
            foreach (var c in commands)
            {
                var commandName = c.Command;
                var cmd = GetCommand<ICommand>(commandName, c.Data as JObject);
                lst.Add(cmd);
            }
            return await Handle(lst.ToArray(), calcTime);
        }

        private T GetCommand<T>(string commandName, JObject command) where T : class, ICommand
        {
            if (string.IsNullOrEmpty(commandName) || !CommandTypes.ContainsKey(commandName))
                return null;

            var type = CommandTypes[commandName];
            if (type == null)
                return null;

            return (command ?? new JObject()).ToObject(type) as T;
        }

        private static readonly Lazy<ConcurrentDictionary<string, Type>> commandTypesLazy =
            new Lazy<ConcurrentDictionary<string, Type>>(GetCommandTypes);

        private static ConcurrentDictionary<string, Type> CommandTypes => commandTypesLazy.Value;

        private static ConcurrentDictionary<string, Type> GetCommandTypes()
        {
            var type = typeof(ICommand);
            var lstTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t != type && type.IsAssignableFrom(t))
                .ToArray();

            var lst = new ConcurrentDictionary<string, Type>();
            foreach (var t in lstTypes)
            {
                lst[t.Name] = t;
            }

            return lst;
        }
    }
}