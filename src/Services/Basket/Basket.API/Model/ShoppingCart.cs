using Basket.API.StrongIdTypes;
using BuildingBlocks.DDD;

namespace Basket.API.Model
{
    public class ShoppingCart : Aggregate<ShoppingCartId>
    {
        public string Username { get; private set; } = default!;
        private readonly List<ShoppingCartItem> _items = new();
        public IReadOnlyList<ShoppingCartItem> Items => _items.AsReadOnly();
        public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);

        public static ShoppingCart Create(Guid id, string username)
        {
            ArgumentNullException.ThrowIfNull(username);

            var cart = new ShoppingCart
            {
                Id = ShoppingCartId.New(id),
                Username = username,
            };
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
                var newItem = new ShoppingCartItem(Id,ticketId, quantity, price);
                _items.Add(newItem);
            }
        }
        public void RemoveItem(Guid ticketId)
        {
            var existingItem = Items.FirstOrDefault(x => x.TicketId == ticketId);
            if(existingItem != null)
            {
                _items.Remove(existingItem);
            }
        }
    }
}
