using Basket.API.Model;

namespace Basket.UnitTests
{
    internal class TestHelper
    {
        public static TicketReadModel CreateTicketSample(Guid id) => new()
        {
            TicketId = id,
            Origin = "Tehran",
            Destination = "Shiraz",
            Description = "Description",
            TravelDate = DateTime.Now.AddDays(5),
            Price = 50000
        };

        public static ShoppingCart CreateUserBasketWithItems(Guid userId)
        {
            var cart = ShoppingCart.Create(userId);

            cart.AddItem(Guid.NewGuid(), 2, 10000);

            return cart;
        }
    }
}
