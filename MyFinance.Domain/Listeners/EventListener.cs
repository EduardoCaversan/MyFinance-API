using MyFinance.Domain.Commands;
using MyFinance.Domain.Utils;

namespace MyFinance.Domain.Events
{
    public class EventListener
    {
        public string Id { get; protected set; }
        public EventListenerType Type { get; }
        protected Func<CommandsHandler, EventListenerType, ICommand, CommandResult, Task> Method { get; set; }

        public EventListener(string id, EventListenerType type, Func<CommandsHandler, EventListenerType, ICommand, CommandResult, Task> method = null)
        {
            Id = id ?? RandomId.New();
            Type = type;
            Method = method;
        }

        public virtual async Task Run(CommandsHandler handler, EventListenerType type, ICommand command, CommandResult result)
        {
            if (Method != null)
                await Method(handler, type, command, result);
        }
    }
}
