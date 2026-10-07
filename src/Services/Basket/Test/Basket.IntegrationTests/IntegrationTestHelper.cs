using Basket.API.Data;
using Basket.API.Model;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.IntegrationTests
{
    public class IntegrationTestHelper(IDistributedCache cache, BasketDbContext basketDbContext)
    {
        public static TicketReadModel BuildTicketReadModel(Guid ticketId, decimal price) =>
           new()
           {
               TicketId = ticketId,
               Origin = "Tehran",
               Destination = "Mashhad",
               Description = "Test ticket",
               TravelDate = DateTime.UtcNow.AddDays(7),
               Price = price
           };

        public async Task ClearCacheAsync(Guid userId)
        {
            await cache.RemoveAsync(userId.ToString());
        }

        public async Task SeedDb(Guid userId)
        {
            await basketDbContext.ShoppingCarts.AddAsync(SeedShoppingCartWithItems(userId));
            await basketDbContext.SaveChangesAsync();

            basketDbContext.ChangeTracker.Clear();
        }
        private ShoppingCart SeedShoppingCartWithItems(Guid testUserId)
        {
            var cart = ShoppingCart.Create(testUserId);
            cart.AddItem(Guid.NewGuid(), 2, 2500);
            cart.AddItem(Guid.NewGuid(), 5, 50000);
            return cart;
        }
    }
}
