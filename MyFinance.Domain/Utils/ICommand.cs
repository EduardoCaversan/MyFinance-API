using MyFinance.Domain.Commands;
using MyFinance.Domain.Events;
using System.Threading.Tasks;

namespace MyFinance.Domain.Utils
{
    public interface ICommand
    {
        Task<CommandResult> GetErrorAsync(CommandsHandler handler);
        Task<bool> HasPermissionAsync(CommandsHandler handler);
        Task<CommandResult> ExecuteAsync(CommandsHandler handler);
        EventListenerType GetEvent();
    }
}