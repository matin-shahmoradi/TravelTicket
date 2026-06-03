using AuthService.Model;

namespace AuthService.Interfaces
{
    public interface IUserSignInManagerService
    {
        Task<bool> ValidateUserPassword(ApplicationUser user, string password);
    }
}
