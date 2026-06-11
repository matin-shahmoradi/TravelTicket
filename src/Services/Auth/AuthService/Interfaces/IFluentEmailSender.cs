using AuthService.Model;
using FluentEmail.Core.Models;

namespace AuthService.Interfaces
{
    public interface IFluentEmailSender
    {
        Task<SendResponse> SendEmailRegisteration(ApplicationUser user, CancellationToken cancellationToken);
    }
}
