using Microsoft.AspNetCore.Identity;

namespace AuthService.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;

        public static ApplicationUser CreateUser(string email, string username, string firstname, string lastname, string phoneNumber)
        {
            return new ApplicationUser
            {
                Email = email,
                UserName = username,
                FirstName = firstname,
                LastName = lastname,
                PhoneNumber = phoneNumber
            };
        }
    }
}
