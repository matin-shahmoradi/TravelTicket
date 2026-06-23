using Basket.API.Common.Dtos.MapExtensions;
using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Data.Repository
{
    public class CachedBasketRepository(
        IBasketRepository basketRepository,
        IDistributedCache cache,
        ICurrentUser currentUser)
        : IBasketRepository
    {
        public async Task<ShoppingCart?> GetBasket(Guid customerId, QueryTrackingBehavior tracking = QueryTrackingBehavior.NoTracking, CancellationToken cancellation = default)
        {
            if (tracking == QueryTrackingBehavior.TrackAll)
            {
                return await basketRepository
                    .GetBasket(
                        customerId: customerId,
                        tracking: QueryTrackingBehavior.TrackAll,
                        cancellation: cancellation
                        );
            }
            var cachedBasketJson = await cache.GetStringAsync(currentUser.UserId.ToString()!, cancellation);

            if (!string.IsNullOrEmpty(cachedBasketJson))
            {
                var cachedbasket = JsonSerializer.Deserialize<ShoppingCartDto>(cachedBasketJson)!;
                return Mapper.MapToShoppingCartEntity(cachedbasket);
            }

            var basket = await basketRepository.GetBasket(
                customerId: customerId,
                tracking: QueryTrackingBehavior.NoTracking,
                cancellation: cancellation);

            if (basket != null)
            {
                var dto = basket.MapToShoppingCartDto();
                await cache.SetStringAsync(customerId.ToString(), JsonSerializer.Serialize(dto));
            }

            return basket;
        }

        public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellation = default)
        {
            var storeBasket = await basketRepository.StoreBasket(basket, cancellation);

            var dto = storeBasket.MapToShoppingCartDto();
            await cache.SetStringAsync(currentUser.UserId.ToString(), JsonSerializer.Serialize(dto));

            return storeBasket;
        }
        public async Task<bool> DeleteBasket(Guid customerId, CancellationToken cancellation = default)
        {
            var deleteBasket = await basketRepository.DeleteBasket(customerId, cancellation);

            await cache.RemoveAsync(customerId.ToString(), cancellation);

            return true;
        }
    }
}
