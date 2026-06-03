using AuthService.Interfaces;
using AuthService.Model;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Repositories
{
    public sealed class UserManagerCommandService(UserManager<ApplicationUser> userManager) : IUserManagerCommandService
    {
        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password) =>
            await userManager.CreateAsync(user, password);

        public async Task<IdentityResult> AddUserToRoleAsync(ApplicationUser user, string role) =>
            await userManager.AddToRoleAsync(user, role);

        public async Task<string> CreateEmailVerificationToken(ApplicationUser user) =>
            await userManager.GenerateEmailConfirmationTokenAsync(user);
    }
}
