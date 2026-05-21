using Microsoft.AspNetCore.Identity;

namespace AuthService.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
    }
}
