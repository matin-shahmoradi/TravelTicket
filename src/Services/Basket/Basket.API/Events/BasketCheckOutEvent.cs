using BuildingBlocks.DDD;

namespace Basket.API.Events
{
    public record BasketCheckOutEvent(
        List<ShoppingCartItemDto> Items) : IDomainEvent;
}
