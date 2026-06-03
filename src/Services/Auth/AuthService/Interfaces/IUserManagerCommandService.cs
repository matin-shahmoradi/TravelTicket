using AuthService.Model;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Interfaces
{
    public interface IUserManagerCommandService
    {
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<IdentityResult> AddUserToRoleAsync(ApplicationUser user, string role);
        Task<string> CreateEmailVerificationToken(ApplicationUser user);
    }
}
