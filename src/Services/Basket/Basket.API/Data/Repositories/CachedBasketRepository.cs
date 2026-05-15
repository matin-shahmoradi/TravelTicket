using Basket.API.Common.Dtos.MapExtensions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Data.Repository
{
    public class CachedBasketRepository(IBasketRepository basketRepository, IDistributedCache cache)
        : IBasketRepository
    {
        public async Task<ShoppingCart> GetBasket(string username, CancellationToken cancellation = default)
        {
            var getCachedBasket = await cache.GetStringAsync(username, cancellation);

            if (!string.IsNullOrEmpty(getCachedBasket))
            {
                var cachedbasket = JsonSerializer.Deserialize<ShoppingCartDto>(getCachedBasket)!;
                return Mapper.MapToShoppingCartEntity(cachedbasket);
            }

            var basket = await basketRepository.GetBasket(username, cancellation);

            if (basket != null)
            {
                var dto = basket.MapToShoppingCartDto();
                await cache.SetStringAsync(username, JsonSerializer.Serialize(dto));
            }

            return basket!;
        }

        public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellation = default)
        {
            var storeBasket = await basketRepository.StoreBasket(basket, cancellation);

            var dto = storeBasket.MapToShoppingCartDto();
            await cache.SetStringAsync(basket.Username, JsonSerializer.Serialize(dto));

            return storeBasket;
        }
        public async Task<bool> DeleteBasket(string username, CancellationToken cancellation = default)
        {
            var deleteBasket = await basketRepository.DeleteBasket(username, cancellation);

            await cache.RemoveAsync(username, cancellation);

            return true;
        }
    }
}
