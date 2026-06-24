using BuildingBlocks.DDD;
using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.Domain.Models
{
    public class OrderItem : Entity<OrderItemId>
    {
        internal OrderItem(OrderId orderId, TicketId ticketId, int quantity, decimal price)
        {
            OrderId = orderId;
            TicketId = ticketId;
            Quantity = quantity;
            Price = price;
        }
        public OrderId OrderId { get; private set; } = default!;
        public TicketId TicketId { get; private set; } = default!;
        public int Quantity { get; private set; } = default!;
        public decimal Price { get; private set; } = default!;
    }
}
