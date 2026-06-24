using BuildingBlocks.DDD;
using Ordering.Domain.Events;
using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.Domain.Models
{
    public class Order : Aggregate<OrderId>
    {
        private readonly List<OrderItem> _orderItems = new();
        public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();

        public CustomerId CustomerId { get; private set; } = default;
        public OrderStatus OrderStatus { get; private set; } = OrderStatus.Pending;
        public decimal TotalPrice
        {
            get => OrderItems.Sum(x => x.Quantity * x.Price);
            private set { }
        }
        public static Order Create(OrderId id, CustomerId customerId)
        {
            var order = new Order
            {
                Id = id,
                CustomerId = customerId,
            };

            order.AddDomainEvents(new OrderCreatedEvent(order));
            return order;
        }

        public void Update(OrderStatus orderStatus)
        {
            var order = new Order
            {
                OrderStatus = orderStatus
            };

            order.AddDomainEvents(new OrderUpdatedEvent(this));
        }

        public void Add(TicketId ticketId, int quantity, decimal price)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
            ArgumentOutOfRangeException.ThrowIfNegative(quantity);

            var orderItem = new OrderItem(Id, ticketId, quantity, price);

            _orderItems.Add(orderItem);
        }

        public void Remove(TicketId ticketId)
        {
            var orderItem = _orderItems.FirstOrDefault(x => x.TicketId == ticketId);
            if (orderItem is not null)
            {
                _orderItems.Remove(orderItem);
            }
        }
    }
}
