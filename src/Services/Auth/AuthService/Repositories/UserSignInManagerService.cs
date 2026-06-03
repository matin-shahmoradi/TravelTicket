using AuthService.Interfaces;
using AuthService.Model;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Repositories
{
    public class UserSignInManagerService(SignInManager<ApplicationUser> signInManager) : IUserSignInManagerService
    {
        public async Task<bool> ValidateUserPassword(ApplicationUser user, string password)
        {
            var signInResult = await signInManager.CheckPasswordSignInAsync(user, password, false);
            return signInResult.Succeeded ? true : false;
        }
    }
}
