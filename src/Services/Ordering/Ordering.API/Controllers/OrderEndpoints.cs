using BuildingBlocks.Extensions;
using Carter;
using MediatR;
using Ordering.Application.Orders.Queries.GetOrderById;

namespace Ordering.API.Controllers
{
    public class OrderEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders/{id:guid}", async (
                HttpContext context,
                ISender sender,
                Guid id) =>
            {
                var command = new GetOrderByIdQuery(id);
                var result = await sender.Send(command);

                if (!result.IsSuccess)
                    return result.ToHttpResult(context);

                return Results.Ok(result);
            });
        }
    }
}
