using BuildingBlocks.Pagination;
using Ordering.Application.Orders.Queries.GetOrders;

namespace Ordering.API.EndPoints
{
    public class GetOrdersEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("orders", async (
                HttpContext context,
                [FromServices] ISender sender,
                [FromQuery] int pageIndex,
                [FromQuery] int pageSize
                ) =>
            {
                var getOrdersQuery = new GetOrdersQuery(new PaginationRequest(pageIndex,pageSize));
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
