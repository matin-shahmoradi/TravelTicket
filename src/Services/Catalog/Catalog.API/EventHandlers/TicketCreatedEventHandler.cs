using BuildingBlocks.Messaging.Events.CatalogEvents;
using MassTransit;

namespace Catalog.API.EventHandlers
{
    public class TicketCreatedEventHandler(
        IBus bus,
        ILogger<TicketCreatedEventHandler> logger) 
        : INotificationHandler<TicketCreatedEvent>
    {
        // TO DO : implement TicketCreatedEventIntegration After learned rabbitmq.
        public async Task Handle(TicketCreatedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain Event handled : {DomainEvent}",notification.GetType().Name);

            var ticketCreatedIntegrationEvent = new TicketCreatedIntegrationEvent
            {
                TicketId = notification.Ticket.Id.Value,
                Origin = notification.Ticket.Origin,
                Destination = notification.Ticket.Destination,
                Description = notification.Ticket.Description,
                TravelDate = notification.Ticket.TravelDate,
                Price = notification.Ticket.Price,
            };

            await bus.Publish(ticketCreatedIntegrationEvent);
        }
    }
}
