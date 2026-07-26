using BuildingBlocks.DDD;
using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.Domain.Models
{
    public class OrderItem : Entity<OrderItemId>
    {
        public OrderId OrderId { get; private set; } = default!;
        public TicketId TicketId { get; private set; } = default!;
        public int Quantity { get; private set; } = default!;
        public decimal Price { get; private set; } = default!;

        public static OrderItem AddItem(OrderId orderId, TicketId ticketId, int quantity, decimal price)
        {
            return new OrderItem
            {
                Id = OrderItemId.New(),
                OrderId = orderId,
                TicketId = ticketId,
                Quantity = quantity,
                Price = price
            };
        }
    }
}
