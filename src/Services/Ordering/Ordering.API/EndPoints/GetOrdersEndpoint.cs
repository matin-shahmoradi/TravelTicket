using BuildingBlocks.Pagination;
using Ordering.Application.Orders.Queries.GetOrders;

namespace Ordering.API.EndPoints
{
    public class GetOrdersEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("orders", async (
                ISender sender,
                PaginationRequest orderRequest,
                HttpContext context
                ) =>
            {
                var getOrdersQuery = new GetOrdersQuery(orderRequest);
                var query = await sender.Send(getOrdersQuery);

                if (!query.IsSuccess)
                {
                    return query.ToProblemResult(context);
                }
                return Results.Ok(query);
            });
        }
    }
}
