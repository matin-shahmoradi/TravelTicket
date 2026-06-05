using AuthService.Model.DTOs.RegisterDtos;
using BuildingBlocks.Extensions;
using Carter;
using MediatR;

namespace AuthService.Auth.Register
{
    public class UserRegisterEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("auth/register", async (
                ISender sender,
                HttpContext httpContext,
                RegisterRequestDto request) =>
            {
                var command = new UserRegisterCommand(request);
                var result = await sender.Send(command);

                if (!result.IsSuccess)
                    return result.ToHttpResult(httpContext);

                return Results.Ok(result);
            })
                .WithName("Register")
                .WithSummary("used for user registration")
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status500InternalServerError);
        }
    }
}
