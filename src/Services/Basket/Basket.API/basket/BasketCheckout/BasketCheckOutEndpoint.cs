namespace Basket.API.basket.BasketCheckout
{
    public class BasketCheckOutEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/baskets/checkout", async (ISender sender, HttpContext context) =>
            {
                var command = new BasketCheckOutCommand();

                var result = await sender.Send(command);

                if (!result.IsSuccess)
                    return result.ToHttpResult(context);

                return Results.Ok(result);
            });
        }
    }
}
