
using Microsoft.AspNetCore.Mvc;
using MyFinance.API.Filters;
using MyFinance.API.v1.Controllers;
using MyFinance.Domain.Commands;
using MyFinance.Domain.Queries;
using MyFinance.Domain.Utils;
using System.Threading.Tasks;

namespace Adventech.Events.API.v1.Controllers
{
    [Route("api/events/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UserTourViewsController : MyFinanceControllerBase
    {
        private readonly CommandsHandler _commandsHandler;
        private readonly QueriesHandler _queriesHandler;

        public UserTourViewsController(CommandsHandler commandsHandler, QueriesHandler queriesHandler)
        {
            _commandsHandler = commandsHandler;
            _queriesHandler = queriesHandler;
        }

        [HttpGet("management")]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<ActionResult<string>> ListUserViewedToursForManagementQuery()
        {
            return "Testing";
        }
    }
}