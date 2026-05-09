using Basket.API.Basket.StoreBasket;

namespace Basket.API.Basket.GetItemFromCache
{
    public class GetItemFromCacheEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("baskets/items/{TicketId}", async (HttpContext context, ISender sender, Guid TicketId) =>
            {
                var command = await sender.Send(new GetItemFromCacheQuery(TicketId));
                if (command.IsSuccess)
                    return Results.Ok(command);

                return command.ToHttpResult(context);
            });
        }
    }
}
