namespace Basket.API.Basket.DeleteBasket
{
    public class DeleteBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("baskets", async (ISender sender , HttpContext context , string travlerNumber) =>
            {
                var command = await sender.Send(new DeleteBasketCommand(travlerNumber));
                if (command.IsSuccess)
                    return Results.Ok(command);

                return command.ToProblemResult(context);
            });
        }
    }
}
