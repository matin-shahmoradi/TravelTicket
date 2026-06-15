using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Basket.AddItemToBasket
{
    public class AddItemToBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("baskets/items", async (
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
            })
                .WithName("AddItemToBasket")
                .WithSummary("used for add item to user basket")
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status500InternalServerError);
        }
    }
}
