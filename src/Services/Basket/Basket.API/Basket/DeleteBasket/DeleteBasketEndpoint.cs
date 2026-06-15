namespace Basket.API.Basket.DeleteBasket
{
    public class DeleteBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("baskets/items", async (ISender sender, HttpContext context) =>
            {
                var command = await sender.Send(new DeleteBasketCommand());
                if (command.IsSuccess)
                    return Results.Ok(command);

                return command.ToHttpResult(context);
            });
        }
    }
}
