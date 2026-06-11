using AuthService.Interfaces;
using AuthService.Model.DTOs.LoginDtos;
using BuildingBlocks;
using BuildingBlocks.CQRS;

namespace AuthService.Auth.Login
{
    internal sealed class LoginCommandHandler(
        IUserManagerQueryService userQueryService,
        IUserRoleQueryService userRoleQueryService,
        IUserSignInManagerService userSignInManager,
        IJsonWebTokenService jsonWebTokenService,
        ILogger<LoginCommandHandler> logger
        )
        : ICommandHandler<LoginCommand, Result<LoginResponse>>
    {
        public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await userQueryService.GetUserByEmailAsync(command.Request.Email, cancellationToken);
            if (user is null)
                return Result<LoginResponse>.Failure(Error.UnAuthorized(message: "invalid username or password"));

            var result = await userSignInManager.ValidateUserPassword(user, command.Request.Password);

            if (!result)
                return Result<LoginResponse>.Failure(Error.UnAuthorized(message: "invalid username or password"));

            var roles = await userRoleQueryService.GetUserRolesAsync(user.Id);

            if (roles is null || roles.Count == 0)
            {
                logger.LogError(
                    "User with Id : {userId} and Email : {userEmail} is registered and dosent have assigned role",
                    user.Id,
                    user.Email);
                return Result<LoginResponse>.Failure(Error.Forbidden());
            }

            var token = jsonWebTokenService.GenerateAccessToken(user, roles);

            logger.LogInformation("User with id: {id} logged in successfully", user.Id);
            return Result<LoginResponse>.Success(new LoginResponse(
                email: user.Email!,
                username: user.UserName!,
                token: token)
                );
        }
    }
}
