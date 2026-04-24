using Ordering.Application.Orders.Commands.CreateOrder;

namespace Ordering.API.EndPoints
{
    public class CreateOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("orders", async (
                HttpContext context,
                [FromServices] ISender sender ,
                [FromBody] OrderDto orderRequest
                ) =>
            {
                var createOrderCommand = new CreateOrderCommand(orderRequest);

                var command = await sender.Send(createOrderCommand);

                if (!command.IsSuccess)
                {
                    return command.ToProblemResult(context);
                }

                return Results.Created($"orders/{command.Value}", command);
            })
                .WithName("CreateOrder")
                .Produces(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Create Order")
                .WithDescription("Create Order");
        }
    }
}
