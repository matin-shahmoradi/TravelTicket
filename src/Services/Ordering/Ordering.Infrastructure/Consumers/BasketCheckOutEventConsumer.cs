using BuildingBlocks.Messaging.Events.BasketEvents;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Dtos;
using Ordering.Application.Orders.Commands.CreateOrder;

namespace Ordering.Application.Events
{
    public sealed class BasketCheckOutEventConsumer(
        ISender sender,
        ILogger<BasketCheckOutEventConsumer> logger)
        : IConsumer<BasketCheckOutIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<BasketCheckOutIntegrationEvent> context)
        {
            logger.LogWarning("[EVENT] EVENT RECEIVED : {event}", context.MessageId);

            Guid id = Guid.NewGuid();
            var item = context.Message.Items;
            var orderDto = new OrderDto
            (
                Id: id,
                OrderStatus: Domain.Enums.OrderStatus.Pending,
                Customer: new CustomerDto(
                    CustomerId: context.Message.CustomerId,
                    nationalCode: "UNKWNOWN",
                    Name: context.Message.Email,
                    Email: context.Message.Email,
                    PhoneNumber: context.Message.PhoneNumber),
                OrderItems: context.Message.Items.Select(x => new OrderItemDto
                (
                    OrderId: id,
                    TicketId: x.TicketId,
                    Quantity: x.Quantity,
                    Price: x.Price
                )).ToList()
            );
            var command = new CreateOrderCommand(orderDto);
            var result = await sender.Send(command);
            if (!result.IsSuccess)
            {
                logger.LogError("[EVENT HANDLED] : Failed to create order : {result}", result.Error);
                return;
            }
            logger.LogInformation("[EVENT HANDLED] : Order Created successfully {result}", result.Value);
        }
    }
}
