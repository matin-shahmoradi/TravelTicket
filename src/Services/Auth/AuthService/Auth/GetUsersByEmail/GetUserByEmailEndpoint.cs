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
            app.MapGet("auth/users/{email}", async (
                ISender sender,
                HttpContext httpContext,
                [FromRoute] string email) =>
            {
                var query = new GetUserByEmailQuery(email);
                var result = await sender.Send(query);

                if (!result.IsSuccess)
                    return result.ToHttpResult(httpContext);

                return Results.Ok(result);
            });
        }
    }
}
