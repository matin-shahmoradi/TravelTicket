using BuildingBlocks.Abstractions;
using BuildingBlocks.Logger;
using BuildingBlocks.Messaging.Events.CatalogEvents;
using Catalog.API.Repository;

namespace Catalog.API.EventHandlers
{
    public class TicketUpdatedEventHandler(
        ITicketCommandRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<TicketUpdatedEventHandler> logger)
        : INotificationHandler<TicketUpdatedEvent>
    {
        // TO DO : implement TicketCreatedEventIntegration After learned rabbitmq.
        public async Task Handle(TicketUpdatedEvent notification, CancellationToken cancellationToken)
        {
            var integration = new TicketUpdatedIntegrationEvent
            {
                TicketId = notification.UpdatedTicket.Id.Value,
                Origin = notification.UpdatedTicket.Origin,
                Destination = notification.UpdatedTicket.Destination,
                Description = notification.UpdatedTicket.Description,
                TravelDate = notification.UpdatedTicket.TravelDate,
            };

            try
            {
                repository.PublishIntegrationEvent(integration);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                logger.LogEventHandler(nameof(TicketUpdatedEvent));
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "[EVENT HANDLER] failed to handle {eventname} : {id}",
                    nameof(TicketUpdatedEvent),
                    notification.UpdatedTicket.Id);
            }
        }
    }
}
