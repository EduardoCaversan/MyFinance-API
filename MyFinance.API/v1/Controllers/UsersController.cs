
using Microsoft.AspNetCore.Mvc;
using MyFinance.API.Filters;
using MyFinance.Domain.Commands;
using MyFinance.Domain.Commands.Users.Commands;
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
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<ActionResult<CommandResult>> ListUsersForSysAdminQuery([FromQuery] ListUsersForSysAdminQuery query)
        {
            return GetResult(await _queriesHandler.RunQuery(query));
        }

        [HttpPost("createNew")]
        public async Task<ActionResult<CommandResult>> CreateNewUserCommand([FromBody] CreateNewUserCommand cmd)
        {
            var result = await _commandsHandler.Handle(cmd);
            return GetResult(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<CommandResult>> AuthenticateUserCommand([FromBody] AuthenticateUserCommand cmd)
        {
            var result = await _commandsHandler.Handle(cmd);
            return GetResult(result);
        }
    }
}