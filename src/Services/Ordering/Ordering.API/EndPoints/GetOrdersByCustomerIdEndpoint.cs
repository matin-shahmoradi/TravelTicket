using Ordering.Application.Orders.Queries.GetOrdersByPhoneNumber;

namespace Ordering.API.EndPoints
{
    public class GetOrdersByCustomerIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("orders/customer/{id:guid}" , async (ISender sender , HttpContext context , [FromRoute] Guid id) =>
            {
                var getOrdersByCustomerId = new GetOrdersByCustomerIdQuery(id);

                var query = await sender.Send(getOrdersByCustomerId);

                if(!query.IsSuccess)
                {
                    return query.ToProblemResult(context);
                }

                return Results.Ok(context);
            })
                .WithName("GetOrdersByCustomerId")
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Get Orders By Customer Id")
                .WithDescription("Get Orders By Customer Id");      
        }
    }
}
