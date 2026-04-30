namespace Basket.API.Basket.StoreBasket
{
    public record StoreBasketCommand(BasketRequest BasketDto) : ICommand<Result<ShoppingCart>>;
    internal sealed class StoreBasketCommandHandler(IBasketRepository basketRepository) : 
        ICommandHandler<StoreBasketCommand, Result<ShoppingCart>>
    {
        public async Task<Result<ShoppingCart>> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
        {
            var basket = ShoppingCart.Create(
                Guid.NewGuid(),
                command.BasketDto.Username);

            basket.AddItem(
                command.BasketDto.TicketId,
                command.BasketDto.Quantity,
                command.BasketDto.Price);

            var basketStore = await basketRepository.StoreBasket(basket,cancellationToken);

            return Result<ShoppingCart>.Success(basketStore);
        }
    }
}
