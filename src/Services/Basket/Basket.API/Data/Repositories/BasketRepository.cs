using Microsoft.EntityFrameworkCore;

namespace Basket.API.Data.Repository
{
    public class BasketRepository(BacketDbContext DbContext)
        : IBasketRepository
    {
        public async Task<ShoppingCart> GetBasket(string username, CancellationToken cancellation)
        {
            var basket = await DbContext
                .ShoppingCarts
                .AsNoTracking()
                .Where(x => x.Username == username)
                .FirstOrDefaultAsync();

            return basket!;
        }
        public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellation)
        {
            await DbContext.ShoppingCarts.AddAsync(basket, cancellation);
            await DbContext.SaveChangesAsync(cancellation);
            return basket;
        }
        public async Task<bool> DeleteBasket(string username, CancellationToken cancellation)
        {
            var getBasket = GetBasket(username , cancellation);

            DbContext.ShoppingCarts.Remove(getBasket.Result);
            await DbContext.SaveChangesAsync(cancellation);
            return true;
        }
    }
}
