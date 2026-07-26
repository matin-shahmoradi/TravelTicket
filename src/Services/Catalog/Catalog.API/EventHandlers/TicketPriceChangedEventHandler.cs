using BuildingBlocks.Infrastracture.Outbox.Extensions;
using BuildingBlocks.Logger;
using BuildingBlocks.Messaging.Events.CatalogEvents;

namespace Catalog.API.EventHandlers
{
    public class TicketPriceChangedEventHandler(
        ICatalogDbContext context,
        ILogger<TicketPriceChangedEventHandler> logger)
        : INotificationHandler<TicketPriceChangedEvent>
    {
        public async Task Handle(TicketPriceChangedEvent notification, CancellationToken cancellationToken)
        {
            var ticketPriceChangedIntegrationEvent = new TicketPriceChangedIntegrationEvent
            {
                TicketId = notification.ticket.Id.Value,
                Origin = notification.ticket.Origin,
                Destination = notification.ticket.Destination,
                Description = notification.ticket.Description,
                TravelDate = notification.ticket.TravelDate,
                Price = notification.ticket.Price,
            };
            try
            {
                context.OutboxMessages.AddIntegrationEvent(ticketPriceChangedIntegrationEvent);
                await context.SaveChangesAsync(cancellationToken);
                logger.LogEventHandler(notification.GetType().Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to handle {event}", notification.GetType().Name);
            }

        }
    }
}
