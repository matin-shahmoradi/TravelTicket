namespace Basket.API.Basket.UpdateItemPriceInBasket
{
    public record UpdateItemPriceBasketCommand(Guid TicketId, decimal Price) : ICommand<Result<bool>>;
}
