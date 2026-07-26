using BuildingBlocks.Logger;
using BuildingBlocks.Messaging.Events.PaymentEvents;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Dtos;
using Ordering.Application.Orders.Commands.UpdateOrder;

namespace Ordering.Infrastructure.Consumers
{
    public sealed class PaymentSucceededEventConsumer(
        ISender sender,
        ILogger<PaymentSucceededEventConsumer> logger)
        : IConsumer<PaymentSucceededIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<PaymentSucceededIntegrationEvent> context)
        {
            logger.LogEventReceived(context.MessageId, nameof(PaymentSucceededEventConsumer));
            var orderDto = new UpdateOrderStatusDto(
                context.Message.OrderId,
                context.Message.PaymentStatus);

            var command = new UpdateOrderCommand(orderDto);
            try
            {
                await sender.Send(command);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "failed to handle {event}", context.GetType().Name);
            }

        }
    }
}
