using BuildingBlocks.Extensions;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Auth.ConfirmEmail
{
    public class ConfirmEmailEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("auth/confirm-email", async (
                ISender sender,
                HttpContext httpContext,
                [FromQuery] string userId,
                [FromQuery] string token) =>
            {
                var command = new ConfirmEmailCommand(userId, token);
                var result = await sender.Send(command);

                if (!result.IsSuccess)
                    return result.ToHttpResult(httpContext);

                return Results.Ok(result);
            });
        }
    }
}
