namespace Basket.API.Basket.GetBasket
{
    public class GetBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("baskets/{username}",async (HttpContext context,ISender sender ,string username) =>
            {
                var getBasket = await sender.Send(new GetBasketQuery(username));
                if(!getBasket.IsSuccess)
                    return getBasket.ToHttpResult(context);

                return Results.Ok(getBasket);
            });
        }
    }
}
