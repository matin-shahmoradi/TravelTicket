using BuildingBlocks.Messaging.Events.PaymentEvents;
using MassTransit;
using MediatR;
using Payment.api.Domain.Events;

namespace Payment.api.EventHandlers
{
    public class PaymentSucceededEventHandler(
        IBus bus,
        ILogger<PaymentSucceededEventHandler> logger) : INotificationHandler<PaymentSucceededDomainEvent>
    {
        public async Task Handle(PaymentSucceededDomainEvent notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new PaymentSucceededIntegrationEvent
            {
                OrderId = notification.OrderId,
                PaymentStatus = notification.PaymentStatus
            };

            try
            {
                await bus.Publish(integrationEvent, cancellationToken);
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
