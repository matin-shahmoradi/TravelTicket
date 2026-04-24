using Ordering.Application.Orders.Queries.GetOrdersByPhoneNumber;

namespace Ordering.API.EndPoints
{
    public class GetOrdersByPhoneNumberEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("orders/customer/{phoneNumber}",
                async (
                    HttpContext context,
                    [FromServices] ISender sender,
                    [FromRoute] string phoneNumber) =>
            {
                var getOrdersByPhoneNumber = new GetOrdersByPhoneNumberQuery(phoneNumber);

                var query = await sender.Send(getOrdersByPhoneNumber);

                if (!query.IsSuccess)
                {
                    return query.ToProblemResult(context);
                }

                return Results.Ok(query);
            })
                .WithName("GetOrdersByPhoneNumber")
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Get Orders By PhoneNumber")
                .WithDescription("Get Orders By PhoneNumber"); ;
        }
    }
}
