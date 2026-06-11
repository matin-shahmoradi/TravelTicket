using AuthService.Interfaces;
using AuthService.Model;
using AuthService.Options;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Text;

namespace AuthService.Repositories
{
    public class FluentEmailSender : IFluentEmailSender
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFluentEmail _fluentEmail;
        private readonly EmailConfirmationUrlOptions _emailConfirmationUrlOptions;
        public FluentEmailSender(
            UserManager<ApplicationUser> userManager,
            IFluentEmail fluentEmail,
            IOptions<EmailConfirmationUrlOptions> emailConfirmationUrlOptions)
        {
            _userManager = userManager;
            _fluentEmail = fluentEmail;
            _emailConfirmationUrlOptions = emailConfirmationUrlOptions.Value;
        }
        public async Task<SendResponse> SendEmailRegisteration(ApplicationUser user, CancellationToken cancellationToken)
        {
            string emailVerificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailVerificationToken));

            string baseUrl = _emailConfirmationUrlOptions.httpUrl;
            string callbackUrl = $"{baseUrl}?userId={user.Id}&token={encodedToken}";

            return await _fluentEmail
                    .To(user.Email)
                    .Subject("Email verification for TravelTicket")
                    .Body($@"Hello {user.FirstName} 
                            to verify your email address click here <a href='{callbackUrl}'>Verify Email</a>", isHtml: true)
                    .SendAsync(cancellationToken);
        }
    }
}
