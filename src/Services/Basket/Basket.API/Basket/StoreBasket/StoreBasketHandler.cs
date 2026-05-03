using Basket.API.Common.Dtos.MapExtensions;
using Basket.API.Data.Repository;

namespace Basket.API.Basket.StoreBasket
{
    public record StoreBasketCommand(BasketRequest BasketDto) : ICommand<Result<ShoppingCartDto>>;
    internal sealed class StoreBasketCommandHandler(IBasketRepository basketRepository) : 
        ICommandHandler<StoreBasketCommand, Result<ShoppingCartDto>>
    {
        public async Task<Result<ShoppingCartDto>> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
        {
            var basket = ShoppingCart.Create(
                command.BasketDto.Username);

            basket.AddItem(
                command.BasketDto.TicketId,
                command.BasketDto.Quantity,
                command.BasketDto.Price);

            var basketStore = await basketRepository.StoreBasket(basket,cancellationToken);

            var dto = basketStore.MapToShoppingCartDto();
            return Result<ShoppingCartDto>.Success(dto);
        }
    }
}
