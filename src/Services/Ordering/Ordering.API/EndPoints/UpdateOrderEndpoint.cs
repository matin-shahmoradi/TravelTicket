using Ordering.Application.Orders.Commands.UpdateOrder;

namespace Ordering.API.EndPoints
{
    public class UpdateOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("orders/{id:guid}" , async (
                HttpContext context,
                [FromServices] ISender sender,
                [FromBody] OrderDto request
                ) =>
            {
                var updateOrderCommand = new UpdateOrderCommand(request);

                var command = await sender.Send(updateOrderCommand);

                if (!command.IsSuccess)
                {
                    return command.ToProblemResult(context);
                }

                return Results.Ok(command);
            })
                .WithName("UpdateOrder")
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Update Order")
                .WithDescription("Update Order"); ;
        }
    }
}
