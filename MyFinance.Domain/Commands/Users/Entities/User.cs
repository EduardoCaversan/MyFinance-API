using MyFinance.Domain.Utils;

namespace MyFinance.Domain.Commands.Users.Entities
{
    public class User : BaseEntity
    {
        public string Id { get; private set; }
        public string Email { get; private set; }
        public string Name { get; private set; }
        public string MobileNumber { get; private set; }
        public string Password { get; private set; }
        public bool AskNewPassword { get; private set; }
        public bool IsSysAdmin { get; private set; }
        public DateTimeOffset CreationDate { get; private set; }
        public DateTimeOffset? LastModified { get; private set; }
        public DateTimeOffset? BirthdayDate { get; private set; }

        public User(
            string email,
            string name,
            string mobileNumber,
            string password,
            DateTimeOffset? birthdayDate = null,
            bool isSysAdmin = false
        )
        {
            Id = RandomId.New();
            Email = email;
            Name = name;
            MobileNumber = mobileNumber;
            Password = password.SHA256Encrypt();
            IsSysAdmin = isSysAdmin;
            CreationDate = DateTimeOffset.Now;
            LastModified = DateTimeOffset.Now;
            BirthdayDate = birthdayDate;
            AskNewPassword = false;
        }

        public void SetNewPassword(string newPassword)
        {
            Password = newPassword.SHA256Encrypt();
            LastModified = DateTimeOffset.UtcNow;
            AskNewPassword = false;
        }

        public void SetAskNewPassword(bool askNewPassword)
        {
            AskNewPassword = askNewPassword;
        }

        public string CreateRecoveryCodeByForgotPassword()
        {
            string recoveryCode = RandomId.NewRandomPassword();
            Password = recoveryCode.SHA256Encrypt();
            AskNewPassword = true;
            return recoveryCode;
        }
    }
}