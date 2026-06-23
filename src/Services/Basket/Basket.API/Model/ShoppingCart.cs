using Basket.API.Events;
using Basket.API.StrongIdTypes;
using BuildingBlocks.DDD;

namespace Basket.API.Model
{
    public class ShoppingCart : Aggregate<ShoppingCartId>
    {
        public Guid CustomerId { get; private set; } = default!;
        private List<ShoppingCartItem> _items = new();
        public IReadOnlyList<ShoppingCartItem> Items => _items.AsReadOnly();
        public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);

        public static ShoppingCart Create(Guid customerId)
        {
            ArgumentNullException.ThrowIfNull(customerId.ToString());

            var cart = new ShoppingCart
            {
                Id = ShoppingCartId.New(),
                CustomerId = customerId
            };
            if (cart.Id.Value == Guid.Empty)
                throw new ArgumentException("shopping cart id cant be empty.");

            return cart;

        }

        public void AddItem(Guid ticketId, int quantity, decimal price)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(quantity);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

            var existingItem = Items.FirstOrDefault(x => x.TicketId == ticketId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var newItem = new ShoppingCartItem(Id, ticketId, quantity, price);
                _items.Add(newItem);
            }
        }
        public void RemoveItem(Guid ticketId)
        {
            var existingItem = Items.FirstOrDefault(x => x.TicketId == ticketId);
            if (existingItem != null)
            {
                _items.Remove(existingItem);
            }
        }

        public void CheckoutBasket()
        {
            if (!_items.Any())
                throw new InvalidOperationException("Cart is empty.");

            AddDomainEvents(new BasketCheckOutEvent(
                Items: _items.Select(x => new ShoppingCartItemDto
                {
                    TicketId = x.TicketId,
                    Price = x.Price,
                    Quantity = x.Quantity
                }).ToList()
            ));
        }
    }
}
