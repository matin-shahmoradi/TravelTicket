namespace Basket.API.Basket.StoreBasket
{
    public record StoreBasketCommand(BasketRequest BasketDto) : ICommand<Result<ShoppingCart>>;
    public class StoreBasketCommandHandler(IBasketRepository basketRepository) : 
        ICommandHandler<StoreBasketCommand, Result<ShoppingCart>>
    {
        public async Task<Result<ShoppingCart>> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
        {
            var shoppingCart = new ShoppingCart()
            {
                Items = command.BasketDto.Items,
                TravlerNumber = command.BasketDto.TravlerNumber,
            };
            var basketStore = await basketRepository.StoreBasket(shoppingCart,cancellationToken);

            return Result<ShoppingCart>.Success(basketStore);
        }
    }
}
