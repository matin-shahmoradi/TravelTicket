using AuthService.Model;
using AuthService.Model.DTOs.RegisterDtos;
using BuildingBlocks;
using BuildingBlocks.CQRS;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace AuthService.Auth.Register
{
    public class UserRegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        IFluentEmail fluentEmail,
        IConfiguration configuration)
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

            string emailVerificationToken = await userManager.GenerateEmailConfirmationTokenAsync(newUser);

            string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailVerificationToken));

            string baseUrl = configuration["EmailConfirmationUrl:httpUrl"]!;
            string callbackUrl = $"{baseUrl}?userId={newUser.Id}&token={encodedToken}";

            var emailResponse = await fluentEmail
                    .To(newUser.Email)
                    .Subject("Email verification for TravelTicket")
                    .Body($@"Hello {newUser.FirstName} 
                            to verify your email address click here <a href='{callbackUrl}'>Verify Email</a>", isHtml: true)
                    .SendAsync(cancellationToken);

            if (!emailResponse.Successful)
            {
                // TODO : Log exceptions.
            }
            var userRegisterResult = new RegisterResponseDto(command.Request.Email, $"{command.Request.Firstname} {command.Request.Lastname}");

            return Result<RegisterResponseDto>.Success(userRegisterResult);
        }
    }
}
