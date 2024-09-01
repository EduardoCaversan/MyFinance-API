
using Microsoft.AspNetCore.Mvc;
using MyFinance.Domain.Commands;
using MyFinance.Domain.Queries;
using MyFinance.Domain.Queries.Users.ListUsersForSysAdminQuery;
using MyFinance.Domain.Utils;

namespace MyFinance.API.v1.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UsersController : MyFinanceControllerBase
    {
        private readonly CommandsHandler _commandsHandler;
        private readonly QueriesHandler _queriesHandler;

        public UsersController(CommandsHandler commandsHandler, QueriesHandler queriesHandler)
        {
            _commandsHandler = commandsHandler;
            _queriesHandler = queriesHandler;
        }

        [HttpGet("management")]
        public async Task<ActionResult<CommandResult>> ListUsersForSysAdminQuery([FromQuery] ListUsersForSysAdminQuery query)
        {
            return GetResult(await _queriesHandler.RunQuery(query));
        }

    }
}