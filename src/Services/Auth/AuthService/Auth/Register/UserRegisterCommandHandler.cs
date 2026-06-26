using AuthService.Interfaces;
using AuthService.Model;
using AuthService.Model.DTOs.RegisterDtos;
using BuildingBlocks;
using BuildingBlocks.CQRS;

namespace AuthService.Auth.Register
{
    public class UserRegisterCommandHandler(
        IUserManagerQueryService userManagerQueryService,
        IUserManagerCommandService userManagerCommandService,
        IFluentEmailSender emailSender,
        ILogger<UserRegisterCommandHandler> logger)
        : ICommandHandler<UserRegisterCommand, Result<RegisterResponseDto>>
    {
        public async Task<Result<RegisterResponseDto>> Handle(UserRegisterCommand command, CancellationToken cancellationToken)
        {
            var userExistCheck = await userManagerQueryService.GetUserByEmailAsync(command.Request.Email, cancellationToken);

            if (userExistCheck is not null)
                return Result<RegisterResponseDto>
                    .Failure(Error.Conflict(message: "Email is already exist."));

            var newUser = ApplicationUser.CreateUser(
                email: command.Request.Email,
                username: command.Request.Email,
                firstname: command.Request.Firstname,
                lastname: command.Request.Lastname,
                phoneNumber: command.Request.PhoneNumber);


            var createUser = await userManagerCommandService.CreateUserAsync(newUser, command.Request.Password);

            if (!createUser.Succeeded)
                return Result<RegisterResponseDto>.Failure(Error.CustomError());


            var addUserToRole = await userManagerCommandService.AddUserToRoleAsync(newUser, Roles.User);
            if (!addUserToRole.Succeeded)
                return Result<RegisterResponseDto>
                    .Failure(Error.Internal_Server(message: $" Cant assign user with id {newUser.Id} to {Roles.User} role"));

            var emailValidationResponse = await emailSender.SendEmailRegisteration(newUser, cancellationToken);

            if (!emailValidationResponse.Successful)
            {
                logger.LogError(
                    "Verification email sending failed. UserId: {UserId}, Email: {Email}, Errors: {@Errors}",
                    newUser.Id,
                    newUser.Email,
                    emailValidationResponse.ErrorMessages
                    );
                return Result<RegisterResponseDto>.Failure(
                    Error.Internal_Server(message: $"failed to send verification email : {emailValidationResponse.ErrorMessages}"));
            }
            var userRegisterResult = new RegisterResponseDto(command.Request.Email, $"{command.Request.Firstname} {command.Request.Lastname}");

            return Result<RegisterResponseDto>.Success(userRegisterResult);
        }
    }
}
