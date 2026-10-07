using Microsoft.EntityFrameworkCore;

namespace Basket.API.Data.Repository
{
    public class BasketRepository(BasketDbContext DbContext)
        : IBasketRepository
    {
        public async Task<ShoppingCart?> GetBasket(Guid customerId, QueryTrackingBehavior tracking, CancellationToken cancellation)
        {
            IQueryable<ShoppingCart> query = DbContext
                .ShoppingCarts
                .Include(x => x.Items)
                .Where(x => x.CustomerId == customerId);

            query = tracking == QueryTrackingBehavior.NoTracking
                ? query.AsNoTracking()
                : query.AsTracking();
            return await query.FirstOrDefaultAsync(cancellation);
        }
        public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellation)
        {
            await DbContext.ShoppingCarts.AddAsync(basket, cancellation);
            await DbContext.SaveChangesAsync(cancellation);
            return basket;
        }
        public async Task<bool> DeleteBasket(Guid customerId, CancellationToken cancellation)
        {
            var deletedCount = await DbContext.ShoppingCarts
                .Where(x => x.CustomerId == customerId)
                .ExecuteDeleteAsync(cancellation);

            return deletedCount > 0;
        }
    }
}
