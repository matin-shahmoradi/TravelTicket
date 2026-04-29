namespace Catalog.API.EventHandlers
{
    public class TicketUpdatedEventHandler(ILogger<TicketUpdatedEventHandler> logger)
        : INotificationHandler<TicketUpdatedEvent>
    {
        // TO DO : implement TicketCreatedEventIntegration After learned rabbitmq.
        public Task Handle(TicketUpdatedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain Event handled : {DomainEvent}", notification.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
