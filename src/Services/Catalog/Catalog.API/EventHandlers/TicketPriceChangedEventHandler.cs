using BuildingBlocks.Messaging.Events.CatalogEvents;
using MassTransit;

namespace Catalog.API.EventHandlers
{
    public class TicketPriceChangedEventHandler
        (IBus bus,ILogger<TicketPriceChangedEventHandler> logger)
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

           await bus.Publish(ticketPriceChangedIntegrationEvent , cancellationToken);
        }
    }
}
