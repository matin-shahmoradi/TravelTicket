namespace Basket.API.Basket.GetBasket
{
    public class GetBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("baskets",async (HttpContext context,ISender sender ,string travlerNumber) =>
            {
                var getBasket = await sender.Send(new GetBasketQuery(travlerNumber));
                if(!getBasket.IsSuccess)
                    return getBasket.ToHttpResult(context);

                return Results.Ok(getBasket);
            });
        }
    }
}
