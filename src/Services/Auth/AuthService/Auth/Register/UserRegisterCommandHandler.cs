using AuthService.Model;
using AuthService.Model.DTOs.RegisterDtos;
using BuildingBlocks;
using BuildingBlocks.CQRS;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace AuthService.Auth.Register
{
    public class UserRegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        IFluentEmail fluentEmail,
        IConfiguration configuration,
        ILogger<UserRegisterCommandHandler> logger)
        : ICommandHandler<UserRegisterCommand, Result<RegisterResponseDto>>
    {
        public async Task<Result<RegisterResponseDto>> Handle(UserRegisterCommand command, CancellationToken cancellationToken)
        {
            var userExistCheck = await userManager.FindByEmailAsync(command.Request.Email);

            if (userExistCheck is not null)
                return Result<RegisterResponseDto>
                    .Failure(Error.UnAuthorized(message: "user is already exist."));

            var newUser = ApplicationUser.CreateUser(
                email: command.Request.Email,
                username: command.Request.Email,
                firstname: command.Request.Firstname,
                lastname: command.Request.Lastname);

            var createUser = await userManager.CreateAsync(newUser, command.Request.Password);

            if (!createUser.Succeeded)
                return Result<RegisterResponseDto>.Failure(Error.CustomError());


            var addUserToRole = await userManager.AddToRoleAsync(newUser, Roles.User);
            if (!addUserToRole.Succeeded)
                return Result<RegisterResponseDto>
                    .Failure(Error.Internal_Server(mesaage: $" Cant assign user with id {newUser.Id} to {Roles.User} role"));

            var emailValidationResponse = await SendValidatationEmail(newUser, cancellationToken);

            if (!emailValidationResponse.Successful)
            {
                logger.LogError(
                    "Verification email sending failed. UserId: {UserId}, Email: {Email}, Errors: {@Errors}",
                    newUser.Id,
                    newUser.Email,
                    emailValidationResponse.ErrorMessages
                    );
            }
            var userRegisterResult = new RegisterResponseDto(command.Request.Email, $"{command.Request.Firstname} {command.Request.Lastname}");

            return Result<RegisterResponseDto>.Success(userRegisterResult);
        }

        private async Task<SendResponse> SendValidatationEmail(ApplicationUser user, CancellationToken cancellationToken)
        {
            string emailVerificationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

            string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailVerificationToken));

            string baseUrl = configuration["EmailConfirmationUrl:httpUrl"]!;
            string callbackUrl = $"{baseUrl}?userId={user.Id}&token={encodedToken}";

            return await fluentEmail
                    .To(user.Email)
                    .Subject("Email verification for TravelTicket")
                    .Body($@"Hello {user.FirstName} 
                            to verify your email address click here <a href='{callbackUrl}'>Verify Email</a>", isHtml: true)
                    .SendAsync(cancellationToken);
        }
    }
}
