using BuildingBlocks.Logger;
using BuildingBlocks.Messaging.Events.PaymentEvents;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Dtos;
using Ordering.Application.Orders.Commands.UpdateOrder;

namespace Ordering.Infrastructure.Consumers
{
    public sealed class PaymentFailedEventConsumer(
        ISender sender,
        ILogger<PaymentFailedEventConsumer> logger)
        : IConsumer<PaymentFailedIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<PaymentFailedIntegrationEvent> context)
        {
            logger.LogEventReceived(context.MessageId, nameof(PaymentFailedEventConsumer));
            var orderDto = new UpdateOrderStatusDto(context.Message.OrderId, context.Message.PaymentStatus);
            var command = new UpdateOrderCommand(orderDto);

            try
            {
                await sender.Send(command);
                logger.LogEventConsumed(context.MessageId, nameof(PaymentFailedEventConsumer));
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "[EVENT HANDLER] Failed to handle event : {event}",
                    context.MessageId);
            }
        }
    }
}
