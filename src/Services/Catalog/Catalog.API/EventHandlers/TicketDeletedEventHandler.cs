using BuildingBlocks.Abstractions;
using BuildingBlocks.Logger;
using BuildingBlocks.Messaging.Events.CatalogEvents;
using Catalog.API.Repository;

namespace Catalog.API.EventHandlers
{
    public sealed class TicketDeletedEventHandler(
        ITicketCommandRepository commandRepository,
        IUnitOfWork unitOfWork,
        ILogger<TicketDeletedEventHandler> logger
        ) : INotificationHandler<TicketDeletedEvent>
    {
        public async Task Handle(TicketDeletedEvent notification, CancellationToken cancellationToken)
        {
            var integration = new TicketDeletedIntegrationEvent
            {
                TicketId = notification.TicketId,
            };

            try
            {
                commandRepository.PublishIntegrationEvent(integration);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                logger.LogEventHandler(nameof(TicketCreatedIntegrationEvent));
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to handle {eventname} event",
                    notification.GetType().Name);
            }
        }
    }
}
