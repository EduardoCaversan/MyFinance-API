using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyFinance.Domain.Commands;
using MyFinance.Domain.Utils;
using MyFinance.Domain.Queries;

namespace MyFinance.API.Filters
{
    public class ValidateTokenFilter : IAsyncActionFilter
    {
        private readonly CommandsHandler _commandsHandler;
        private readonly QueriesHandler _queriesHandler;

        public ValidateTokenFilter(CommandsHandler commandsHandler, QueriesHandler queriesHandler)
        {
            _commandsHandler = commandsHandler;
            _queriesHandler = queriesHandler;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var token = "";

            if (context.HttpContext.Request.Headers.ContainsKey("Authorization"))
                token = Convert.ToString(context.HttpContext.Request.Headers["Authorization"]);

            if (string.IsNullOrWhiteSpace(token) || token.Length < 15 || !token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new UnauthorizedObjectResult(new { Message = "Cabeçalho do token é inválido." });
                return;
            }

            try
            {
                var tk = new TokenData(token);
                var isValid = await tk.ValidateAsync();

                if (!isValid)
                {
                    context.Result = new UnauthorizedObjectResult(new { Message = "Dado do token é inválido." });
                    return;
                }
                else
                {
                    var ipAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
                    var url = context.HttpContext.Request.Path.ToString();
                    var method = context.HttpContext.Request.Method;
                    var contextRequestData = new RequestData(
                        tk.UserId,
                        tk.UserIsAdmin,
                        token,
                        ipAddress,
                        method,
                        url
                    );

                    _commandsHandler.CurrentRequest = contextRequestData;
                    _queriesHandler.CurrentRequest = contextRequestData;
                }
                
                await next();
            }
            catch (Exception)
            {
                context.Result = new UnauthorizedObjectResult(new { Message = "Token é inválido." });
                return;
            }
        }
    }
}