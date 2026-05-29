using AuthService.Model.DTOs.LoginDtos;
using BuildingBlocks;
using BuildingBlocks.CQRS;

namespace AuthService.Auth.Login
{
    public record LoginCommand(LoginRequest Request) : ICommand<Result<LoginResponse>>;
}
