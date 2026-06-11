using AuthService.Interfaces;
using BuildingBlocks;
using BuildingBlocks.CQRS;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace AuthService.Auth.ConfirmEmail
{
    internal sealed class ConfirmEmailCommandHandler(
        IUserManagerCommandService userManagerCommandService,
        IUserManagerQueryService userManagerQueryService)
        : ICommandHandler<ConfirmEmailCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
        {
            var user = await userManagerQueryService.GetUserByIdAsync(command.UserId, cancellationToken);
            if (user is null)
                return Result<string>.Failure(Error.NotFoundError(message: "User not found!"));

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(command.EmailConfirmationToken));

            var result = await userManagerCommandService.ConfirmUserEmailAsync(user, decodedToken);

            if (!result.Succeeded)
                return Result<string>.Failure(Error.Internal_Server(message: $"{result.Errors}"));

            return Result<string>.Success($"Your email confirmed successfully");
        }
    }
}
