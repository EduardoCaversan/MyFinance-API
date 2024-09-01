using Microsoft.EntityFrameworkCore;
using MyFinance.Domain.Commands.Users.Entities;
using MyFinance.Domain.Events;
using MyFinance.Domain.Utils;

namespace MyFinance.Domain.Commands.Users.Commands
{
    public class CreateNewUserCommand : ICommand
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public DateTimeOffset? BirthdayDate { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public async Task<CommandResult> GetErrorAsync(CommandsHandler handler)
        {
            if (string.IsNullOrWhiteSpace(Name))
                return await Task.FromResult(new CommandResult(ErrorCode.InvalidParameters, "O nome é obrigatório!"));
            if (string.IsNullOrWhiteSpace(Email))
                return await Task.FromResult(new CommandResult(ErrorCode.InvalidParameters, "O email é obrigatório!"));
            if (string.IsNullOrWhiteSpace(MobileNumber))
                return await Task.FromResult(new CommandResult(ErrorCode.InvalidParameters, "O WhatsApp para contato é obrigatório!"));
            if (!Validations.IsMobileNumber(MobileNumber))
                return await Task.FromResult(new CommandResult(ErrorCode.InvalidParameters, "O WhatsApp para contato deve ser válido!"));
            if (string.IsNullOrWhiteSpace(Password))
                return await Task.FromResult(new CommandResult(ErrorCode.InvalidParameters, "A senha é obrigatória!"));
            if (string.IsNullOrWhiteSpace(PasswordConfirmation))
                return await Task.FromResult(new CommandResult(ErrorCode.InvalidParameters, "Confirme sua senha!"));
            if (Password != PasswordConfirmation)
                return await Task.FromResult(new CommandResult(ErrorCode.InvalidParameters, "As senhas não são iguais!"));


            var thereAreSameEmailUser = await handler.DbContext.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == Email);

            if (thereAreSameEmailUser)
                return await Task.FromResult(new CommandResult(ErrorCode.InvalidParameters, $"Já existe uma conta com o email: {Email}!"));

            return null;
        }
        public async Task<bool> HasPermissionAsync(CommandsHandler handler)
        {
            return await Task.FromResult(true);
        }

        public async Task<CommandResult> ExecuteAsync(CommandsHandler handler)
        {
            await handler.DbContext.Users.AddAsync(new User(
                name: Name,
                email: Email,
                mobileNumber: MobileNumber,
                password: Password,
                birthdayDate: BirthdayDate
            ));


            var rows = await handler.DbContext.SaveChangesAsync();
            return await Task.FromResult(new CommandResult(rows));
        }

        public EventListenerType GetEvent()
        {
            return EventListenerType.None;
        }
    }
}
