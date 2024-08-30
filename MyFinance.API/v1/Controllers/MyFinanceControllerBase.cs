using Microsoft.AspNetCore.Mvc;
using MyFinance.Domain.Utils;

namespace MyFinance.API.v1.Controllers
{
    public abstract class MyFinanceControllerBase : ControllerBase
    {
        protected ActionResult GetResult<T>(QueryResult<T> queryResult, bool onlyOne = false) where T : class, IViewModel
        {
            return queryResult.ErrorCode switch
            {
                ErrorCode.NotFound => NotFound(queryResult),
                ErrorCode.InvalidParameters => BadRequest(queryResult),
                ErrorCode.Unauthorized => StatusCode(403, queryResult),
                ErrorCode.OnlySysAdminAccess => StatusCode(403, queryResult),
                _ => onlyOne ?
                                Ok(queryResult?.FirstOrDefault()) :
                                Ok(queryResult),
            };
        }

        protected ActionResult GetResult(CommandResult commandResult)
        {
            return commandResult.ErrorCode switch
            {
                ErrorCode.NotFound => NotFound(commandResult),
                ErrorCode.InvalidParameters => BadRequest(commandResult),
                ErrorCode.NotAllowedCommand => BadRequest(commandResult),
                ErrorCode.DuplicateUniqueIdentifier => Conflict(commandResult),
                ErrorCode.Unauthorized => StatusCode(403, commandResult),
                ErrorCode.OnlySysAdminAccess => StatusCode(403, commandResult),
                _ => Ok(commandResult),
            };
        }
    }
}
