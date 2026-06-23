using Microsoft.EntityFrameworkCore;

namespace Basket.API.Data.Repository
{
    public interface IBasketRepository
    {
        Task<ShoppingCart?> GetBasket(Guid customerId, QueryTrackingBehavior tracking = QueryTrackingBehavior.NoTracking, CancellationToken cancellation = default);
        Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellation = default);
        Task<bool> DeleteBasket(Guid customerId, CancellationToken cancellation = default);
    }
}
