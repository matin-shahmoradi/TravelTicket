using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Data
{
    public class CachedBasketRepository(IBasketRepository basketRepository , IDistributedCache cache) 
        : IBasketRepository
    {
        public async Task<ShoppingCart> GetBasket(string travlerNumber, CancellationToken cancellation = default)
        {
            var getCachedBasket = await cache.GetStringAsync(travlerNumber , cancellation);

            if (!string.IsNullOrEmpty(getCachedBasket))
               return JsonSerializer.Deserialize<ShoppingCart>(getCachedBasket)!;

            var basket = await basketRepository.GetBasket(travlerNumber,cancellation);

            await cache.SetStringAsync(travlerNumber, JsonSerializer.Serialize(basket));

            return basket;
        }

        public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellation = default)
        {
            var storeBasket = await basketRepository.StoreBasket(basket, cancellation);

            await cache.SetStringAsync(basket.TravlerNumber,JsonSerializer.Serialize(basket));

            return storeBasket;
        }
        public async Task<bool> DeleteBasket(string travlerNumber, CancellationToken cancellation = default)
        {
            var deleteBasket = await basketRepository.DeleteBasket(travlerNumber, cancellation);

            await cache.RemoveAsync(travlerNumber,cancellation);

            return true;
        }
    }
}
