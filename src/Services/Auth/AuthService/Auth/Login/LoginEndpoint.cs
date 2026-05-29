using AuthService.Model.DTOs.LoginDtos;
using BuildingBlocks.Extensions;
using Carter;
using MediatR;

namespace AuthService.Auth.Login
{
    public class LoginEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("auth/login", async (
                ISender sender,
                HttpContext httpContext,
                LoginRequest request) =>
            {
                var command = new LoginCommand(request);
                var result = await sender.Send(command);
                if (!result.IsSuccess)
                {
                    return result.ToHttpResult(httpContext);
                }

                return Results.Ok(result);
            });
        }
    }
}
