using AuthService.Model;

namespace AuthService.Interfaces
{
    public interface IJsonWebTokenService
    {
        string GenerateAccessToken(ApplicationUser user, IReadOnlyList<string> roles);
    }
}
