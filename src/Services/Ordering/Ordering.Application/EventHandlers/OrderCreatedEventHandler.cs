using BuildingBlocks.Infrastracture.Outbox.Extensions;
using BuildingBlocks.Logger;
using BuildingBlocks.Messaging.Events.OrderEvents;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Events;

namespace Ordering.Application.EventHandlers
{
    internal sealed class OrderCreatedEventHandler(
        IOrderDbContext orderDbContext,
        ILogger<OrderCreatedEventHandler> logger)
        : INotificationHandler<OrderCreatedEvent>
    {
        public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new OrderCreatedIntegrationEvent
            {
                OrderId = notification.Order.Id.Value,
                CustomerId = notification.Order.CustomerId.Value,
                Amount = notification.Order.TotalPrice
            };
            try
            {
                orderDbContext.OutboxMessages.AddIntegrationEvent(integrationEvent);
                await orderDbContext.SaveChangesAsync(cancellationToken);
                logger.LogEventHandler(integrationEvent.GetType().Name);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "[EVENT] Failed to handle {event}",
                    integrationEvent.GetType().Name);
            }
        }
    }
}
