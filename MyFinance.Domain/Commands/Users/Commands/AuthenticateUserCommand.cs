using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyFinance.Domain.Commands.Users.Entities;
using MyFinance.Domain.Events;
using MyFinance.Domain.Utils;

namespace MyFinance.Domain.Commands.Users.Commands
{
    public class AuthenticateUserCommand : ICommand
    {
        public string Email { get; set; }
        public string Password { get; set; }

        private User User;
        public async Task<CommandResult> GetErrorAsync(CommandsHandler handler)
        {
            if (string.IsNullOrWhiteSpace(Email))
                return await Task.FromResult(new CommandResult(ErrorCode.InvalidParameters, "O email é obrigatório!"));
            if (string.IsNullOrWhiteSpace(Password))
                return await Task.FromResult(new CommandResult(ErrorCode.InvalidParameters, "A senha é obrigatória!"));

            string encryptedPassword = Password.SHA256Encrypt();
            User = await handler.DbContext.Users.FirstOrDefaultAsync(u => u.Email == Email && u.Password == encryptedPassword);

            if (User == null)
                return await Task.FromResult(new CommandResult(ErrorCode.InvalidParameters, $"O email ou a senha estão inválidos, verifique e tente novamente!"));

            return null;
        }
        public async Task<bool> HasPermissionAsync(CommandsHandler handler)
        {
            return await Task.FromResult(true);
        }

        public async Task<CommandResult> ExecuteAsync(CommandsHandler handler)
        {
            var token = GenerateToken(User.Id, User.IsSysAdmin, DateTime.UtcNow.AddMinutes(60 * 10));
            return await Task.FromResult(new CommandResult(0, resultData: new { Token = token}));
        }

        public static string GenerateToken(string userId, bool userIsAdmin, DateTime expirationDate)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim("UserIsAdmin", userIsAdmin.ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(expirationDate).ToUnixTimeSeconds().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Keys.ACCESS_TOKEN_SIGNATURE));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Keys.Issuer,
                audience: "MyFinance",
                claims: claims,
                expires: expirationDate,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public EventListenerType GetEvent()
        {
            return EventListenerType.None;
        }
    }
}
