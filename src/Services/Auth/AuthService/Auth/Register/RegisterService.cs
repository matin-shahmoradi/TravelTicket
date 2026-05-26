using AuthService.Model;
using AuthService.Model.DTOs.RegisterDtos;
using BuildingBlocks;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Auth.Register
{
    public class RegisterService(UserManager<ApplicationUser> userManager)
    {
        public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            var userExistCheck = await userManager.FindByEmailAsync(request.Email);

            if (userExistCheck is not null)
                return Result<RegisterResponse>
                    .Failure(Error.UnAuthorized(message: "user is already exist."));

            var newUser = ApplicationUser.CreateUser(
                email: request.Email,
                username: request.Email,
                firstname: request.Firstname,
                lastname: request.Lastname);

            var createUser = await userManager.CreateAsync(newUser,request.Password);
            var addUserToRole = await userManager.AddToRoleAsync(newUser, Roles.User);

            if (!createUser.Succeeded)
                return Result<RegisterResponse>.Failure(Error.CustomError());

            var userRegisterResult = new RegisterResponse(request.Email, $"{request.Firstname} {request.Lastname}");

            return Result<RegisterResponse>.Success(userRegisterResult);
        }
    }
}
