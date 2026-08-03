using BuildingBlocks.Abstractions;
using BuildingBlocks.Logger;
using BuildingBlocks.Messaging.Events.CatalogEvents;
using Catalog.API.Repository;

namespace Catalog.API.EventHandlers
{
    public class TicketPriceChangedEventHandler(
        ITicketCommandRepository repository,
        IUnitOfWork unitOfWork,
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
                repository.PublishIntegrationEvent(ticketPriceChangedIntegrationEvent);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                logger.LogEventHandler(notification.GetType().Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to handle {event}", notification.GetType().Name);
            }

        }
    }
}
