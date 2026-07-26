using BuildingBlocks.Messaging.Events.PaymentEvents;
using MassTransit;
using MediatR;
using Payment.api.Domain.Events;

namespace Payment.api.EventHandlers
{
    public class PaymentFailedEventHandler(
        IBus bus,
        ILogger<PaymentFailedEventHandler> logger) : INotificationHandler<PaymentFailedDomainEvent>
    {
        public async Task Handle(PaymentFailedDomainEvent notification, CancellationToken cancellationToken)
        {
            var integration = new PaymentFailedIntegrationEvent
            {
                OrderId = notification.OrderId,
                PaymentStatus = notification.PaymentStatus.ToString(),
            };

            try
            {
                await bus.Publish(integration);
                logger.LogInformation("[EVENT] {event} Published", notification.GetType().Name);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "[Event] Failed to handle {event}",
                    notification.GetType().Name);
            }
        }
    }
}
