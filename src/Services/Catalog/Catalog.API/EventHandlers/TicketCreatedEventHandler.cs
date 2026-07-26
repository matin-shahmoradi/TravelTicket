using BuildingBlocks.Infrastracture.Outbox.Extensions;
using BuildingBlocks.Logger;
using BuildingBlocks.Messaging.Events.CatalogEvents;

namespace Catalog.API.EventHandlers
{
    public class TicketCreatedEventHandler(
        ICatalogDbContext context,
        ILogger<TicketCreatedEventHandler> logger)
        : INotificationHandler<TicketCreatedEvent>
    {
        public async Task Handle(TicketCreatedEvent notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new TicketCreatedIntegrationEvent
            {
                TicketId = notification.Ticket.Id.Value,
                Origin = notification.Ticket.Origin,
                Destination = notification.Ticket.Destination,
                Description = notification.Ticket.Description,
                TravelDate = notification.Ticket.TravelDate,
                Price = notification.Ticket.Price,
            };
            try
            {
                context.OutboxMessages.AddIntegrationEvent(integrationEvent);
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
