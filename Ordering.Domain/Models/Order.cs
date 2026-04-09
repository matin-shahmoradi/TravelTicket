using Ordering.Domain.Events;
using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.Domain.Models
{
    public class Order : Aggregate<OrderId>
    {
        private readonly List<OrderItem> _orderItems = new();
        public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();

        public CustomerId CustomerId { get; private set; } = default!;

        public OrderName OrderName { get; private set; } = default!;
        public Payment Payment { get; private set; } = default!;
        public OrderStatus OrderStatus { get; private set; } = OrderStatus.Pending;
        public decimal TotalPrice
        {
            get => OrderItems.Sum(x => x.Quantity * x.Price);
            private set { }
        }
        public static Order Create(OrderId id , CustomerId customerId, OrderName orderName,OrderStatus orderStatus , Payment payment)
        {
            var order = new Order
            {
                Id = id,
                CustomerId = customerId,
                OrderName = orderName,
                OrderStatus = OrderStatus.Pending,
                Payment = payment
            };

            order.AddDomainEvents(new OrderCreatedEvent(order));
            return order;
        }

        public static void Update(OrderName orderName , OrderStatus orderStatus, Payment payment)
        {
            var order = new Order
            {
                OrderName = orderName,
                OrderStatus = orderStatus,
                Payment = payment
            };

            order.AddDomainEvents(new OrderUpdatedEvent(order));
        }

        //public static void Add(TicketId ticketId , int quantity , decimal price)
        //{
        //    var orderItem = new OrderItem(orderId:,ticketId,quantity,price);
        //}

    }
}
