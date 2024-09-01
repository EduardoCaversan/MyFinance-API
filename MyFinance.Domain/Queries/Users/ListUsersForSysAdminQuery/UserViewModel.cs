using MyFinance.Domain.Utils;

namespace MyFinance.Domain.Queries.Users.ListUsersForSysAdminQuery
{
    public class UserViewModel : IViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
    }
}