using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Basket.AddItemToBasket
{
    public class AddItemToBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("baskets/{username}/items", async (
                HttpContext context,
                ISender sender, 
                [FromBody] AddItemToBasketDto dto) =>
            {
                var command = new AddItemToBasketCommand(dto);

                var addItemToBasket = await sender.Send(command);

                if (!addItemToBasket.IsSuccess)
                {
                    return addItemToBasket.ToHttpResult(context);
                }
                // TODO : Redirect user to GetBasket Endpoint.
                return Results.Ok(addItemToBasket);
            });
        }
    }
}
