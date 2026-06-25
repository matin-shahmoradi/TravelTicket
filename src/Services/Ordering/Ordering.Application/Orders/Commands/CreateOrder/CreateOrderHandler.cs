using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.Application.Orders.Commands.CreateOrder
{
    public sealed class CreateOrderHandler(
        IOrderDbContext orderContext,
        ILogger<CreateOrderHandler> logger)
        : ICommandHandler<CreateOrderCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var order = CreateNewOrder(command.OrderDto);
            var customer = await orderContext.Customers
                .Where(x => x.Id == order.Item2.Id)
                .FirstOrDefaultAsync(cancellationToken);
            try
            {
                await orderContext.Orders.AddAsync(order.Item1);
                await orderContext.Customers.AddAsync(order.Item2);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to create new order. error : {error} ", e.Message);
                return Result<Guid>.Failure(Error.Internal_Server(message: e.Message));
            }
            await orderContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Order created successfully : {order}", order);
            return Result<Guid>.Success(command.OrderDto.Id);
        }

        private (Order, Customer) CreateNewOrder(OrderDto orderDto)
        {
            var newOrder = Order.Create(
                id: OrderId.New(),
                customerId: CustomerId.New(orderDto.Customer.CustomerId));

            foreach (var orderItemDto in orderDto.OrderItems)
            {
                newOrder.Add(
                    ticketId: TicketId.New(orderItemDto.TicketId),
                    quantity: orderItemDto.Quantity,
                    price: orderItemDto.Price);
            }

            var customer = Customer.Create(
                id: CustomerId.New(orderDto.Customer.CustomerId),
                name: orderDto.Customer.Name!,
                nationalCode: orderDto.Customer.nationalCode!,
                phoneNumber: orderDto.Customer.PhoneNumber!,
                email: orderDto.Customer.Email!);

            return (newOrder, customer);
        }
    }
}
