using BuildingBlocks.Extensions;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Auth.GetUsersByEmail
{
    public class GetUserByEmailEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("auth/users", async (
                ISender sender,
                HttpContext httpContext,
                [FromQuery] string email) =>
            {
                var query = new GetUserByEmailQuery(email);
                var result = await sender.Send(query);

                if (!result.IsSuccess)
                    return result.ToHttpResult(httpContext);

                return Results.Ok(result);
            })
                .WithName("GetUserByEmail")
                .WithSummary("Retrieve user details by email")
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status500InternalServerError)
                .RequireAuthorization("AdminOnly");
        }
    }
}
