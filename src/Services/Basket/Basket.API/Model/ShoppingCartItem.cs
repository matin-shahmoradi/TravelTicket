using Basket.API.StrongIdTypes;
using BuildingBlocks.DDD;

namespace Basket.API.Model
{
    public class ShoppingCartItem : Entity<Guid>
    {
        public ShoppingCartId ShoppingCartId { get; private set; }
        public Guid TicketId { get; private set; }
        public int Quantity { get; internal set; }
        public decimal Price { get; private set; }

        internal ShoppingCartItem(ShoppingCartId shoppingCartId, Guid ticketId, int quantity, decimal price)
        {
            ShoppingCartId = shoppingCartId;
            TicketId = ticketId;
            Quantity = quantity;
            Price = price;
        }
    }
}
