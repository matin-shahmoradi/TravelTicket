using BuildingBlocks;

namespace AuthService.Interfaces
{
    public interface IMailKitEmailSender
    {
        Task<Result<string>> SendEmail(string mail, string receiver, CancellationToken cancellationToken = default);
    }
}
