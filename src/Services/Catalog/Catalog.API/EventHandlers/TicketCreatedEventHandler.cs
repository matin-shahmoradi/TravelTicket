using BuildingBlocks.Abstractions;
using BuildingBlocks.Logger;
using BuildingBlocks.Messaging.Events.CatalogEvents;
using Catalog.API.Repository;

namespace Catalog.API.EventHandlers
{
    public class TicketCreatedEventHandler(
        ITicketCommandRepository repository,
        IUnitOfWork unitOfWork,
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
                repository.PublishIntegrationEvent(integrationEvent);
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
