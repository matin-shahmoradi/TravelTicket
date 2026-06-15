namespace Basket.API.Basket.GetBasket
{
    public class GetBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/baskets", async (HttpContext context, ISender sender) =>
            {
                var getBasket = await sender.Send(new GetBasketQuery());
                if (!getBasket.IsSuccess)
                    return getBasket.ToHttpResult(context);

                return Results.Ok(getBasket);
            })
                .WithName("GetBasket")
                .WithSummary("Get user basket")
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status500InternalServerError)
                .RequireAuthorization("User");
        }
    }
}
