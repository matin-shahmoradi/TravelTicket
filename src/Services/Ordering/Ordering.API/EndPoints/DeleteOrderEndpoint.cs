using Ordering.Application.Orders.Commands.DeleteOrder;
using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.API.EndPoints
{
    public class DeleteOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("orders/{id:guid}" , async (ISender sender ,[FromRoute] Guid id , HttpContext context) =>
            {
                var deleteOrderCommand = new DeleteOrderCommand(OrderId.New(id));

                var command = await sender.Send(deleteOrderCommand);

                if (!command.IsSuccess)
                {
                    return command.ToProblemResult(context);
                }

                return Results.Ok(command);
            })
                .WithName("DeleteOrder")
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Delete Order")
                .WithDescription("Delete Order");
        }
    }
}
