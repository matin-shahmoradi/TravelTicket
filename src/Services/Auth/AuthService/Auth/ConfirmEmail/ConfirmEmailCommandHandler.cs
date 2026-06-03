using AuthService.Model;
using BuildingBlocks;
using BuildingBlocks.CQRS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace AuthService.Auth.ConfirmEmail
{
    internal sealed class ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager) : ICommandHandler<ConfirmEmailCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(command.UserId);
            if (user is null)
                return Result<string>.Failure(Error.NotFoundError(message: "User not found!"));

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(command.EmailConfirmationToken));

            var result = await userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
                return Result<string>.Failure(Error.CustomError(mesaage: $"{result.Errors}"));

            return Result<string>.Success($"Your email confirmed successfully");
        }
    }
}
