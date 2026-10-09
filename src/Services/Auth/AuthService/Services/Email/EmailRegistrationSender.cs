using AuthService.Interfaces;
using AuthService.Model;
using AuthService.Options;
using BuildingBlocks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Text;

namespace AuthService.Services.Email
{
    public class EmailRegistrationSender(
        IUserManagerCommandService UserManagerCommand,
        IMailKitEmailSender EmailSender,
        IOptions<EmailConfirmationUrlOptions> Options) : IUserRegistrationEmailSender
    {
        public async Task<Result<string>> SendEmailRegistration(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            var emailVerificationToken = await UserManagerCommand.CreateEmailVerificationToken(user);

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailVerificationToken));

            string baseUrl = Options.Value.httpUrl;
            string callbackUrl = $"{baseUrl}?userId={user.Id}&token={encodedToken}";


            string verificationEmail = $@"Hello {user.FirstName} 
                            to verify your email address click here <a href='{callbackUrl}'>Verify Email</a>";

            return await EmailSender.SendEmail(verificationEmail, user.Email!, cancellationToken);
        }
    }
}
