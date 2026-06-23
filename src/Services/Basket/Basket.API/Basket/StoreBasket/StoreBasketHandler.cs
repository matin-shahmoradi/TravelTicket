using Basket.API.Common.Dtos.MapExtensions;
using Basket.API.Data.Repositories;
using BuildingBlocks.Abstractions;
using System.Text.Json;

namespace Basket.API.Basket.StoreBasket
{
    public record StoreBasketCommand(BasketRequest BasketDto) : ICommand<Result<ShoppingCartDto>>;
    internal sealed class StoreBasketCommandHandler(
        IBasketRepository basketRepository,
        ICacheTicketRepository ticketRepository,
        ICurrentUser currentUser) :

        ICommandHandler<StoreBasketCommand, Result<ShoppingCartDto>>
    {
        public async Task<Result<ShoppingCartDto>> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
        {
            var basket = await basketRepository.GetBasket(
               customerId: currentUser.UserId!,
               cancellation: cancellationToken);

            if (basket is null)
            {
                basket = ShoppingCart.Create(currentUser.UserId!);
            }

            var getTicketFromCache = await ticketRepository.ReadTicketFromCacheAsync(
                command.BasketDto.TicketId.ToString(),
                cancellationToken);

            if (getTicketFromCache is not null)
            {
                var price = JsonSerializer.Deserialize<TicketReadModel>(getTicketFromCache)!;
                basket.AddItem(
                    command.BasketDto.TicketId,
                    command.BasketDto.Quantity,
                    price.Price);
            }
            // TODO : if cache miss
            // - Get data from catalog service using gRPC.

            var basketStore = await basketRepository.StoreBasket(basket, cancellationToken);

            var dto = basketStore.MapToShoppingCartDto();
            return Result<ShoppingCartDto>.Success(dto);
        }
    }
}
