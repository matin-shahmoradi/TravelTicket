namespace Catalog.API.EventHandlers
{
    public class TicketCreatedEventHandler(ILogger<TicketCreatedEventHandler> logger) 
        : INotificationHandler<TicketCreatedEvent>
    {
        // TO DO : implement TicketCreatedEventIntegration After learned rabbitmq.
        public Task Handle(TicketCreatedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain Event handled : {DomainEvent}",notification.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
