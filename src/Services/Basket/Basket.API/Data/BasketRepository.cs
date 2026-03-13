namespace Basket.API.Data
{
    public class BasketRepository(IDocumentSession session)
        : IBasketRepository
    {
        public async Task<ShoppingCart> GetBasket(string travlerNumber, CancellationToken cancellation)
        {
            var basket = await session.LoadAsync<ShoppingCart>(travlerNumber);
            return basket!;
        }
        public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellation)
        {
            session.Store(basket);
            await session.SaveChangesAsync();
            return basket;
        }
        public async Task<bool> DeleteBasket(string travlerNumber, CancellationToken cancellation)
        {
            session.Delete<ShoppingCart>(travlerNumber);
            await session.SaveChangesAsync();
            return true;
        }
    }
}
