namespace Basket.API.Basket.StoreBasket
{
    public class StoreBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("baskets",async (HttpContext context, ISender sender, BasketRequest request) =>
            {
                var command = await sender.Send(new StoreBasketCommand(request));
                if (command.IsSuccess)
                    return Results.Created($"baskets/{command.Value}",command.Value);

                return command.ToHttpResult(context);
            });
        }
    }
}
