using AuthService.Model;
using BuildingBlocks;

namespace AuthService.Interfaces
{
    public interface IUserRegistrationEmailSender
    {
        Task<Result<string>> SendEmailRegistration(ApplicationUser user, CancellationToken cancellationToken = default);
    }
}
