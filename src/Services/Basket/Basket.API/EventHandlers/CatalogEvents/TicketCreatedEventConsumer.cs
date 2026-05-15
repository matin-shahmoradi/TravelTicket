using Basket.API.Basket.StoreTicket;
using BuildingBlocks.Messaging.Events.CatalogEvents;
using MassTransit;

namespace Basket.API.EventHandlers.CatalogEvents
{
    public class TicketCreatedEventConsumer(
        ISender sender,
        ILogger<TicketCreatedEventConsumer> logger
        )
        : IConsumer<TicketCreatedIntegrationEvent>

    {
        public async Task Consume(ConsumeContext<TicketCreatedIntegrationEvent> context)
        {
            logger.LogWarning("EVENT RECEIVED {TicketId}", context.Message.TicketId);

            var ticketReadModel = new TicketReadModel
            {
                TicketId = context.Message.TicketId,
                Origin = context.Message.Origin,
                Destination = context.Message.Destination,
                Description = context.Message.Description,
                TravelDate = context.Message.TravelDate,
                Price = context.Message.Price
            };

            var command = new StoreTicketCommand(ticketReadModel);
            var sendToHandler = await sender.Send(command);

            if (!sendToHandler.IsSuccess)
            {
                logger.LogError("Failed cache ticket in Redis. Ticket id : {TicketId}",context.Message.TicketId);
            }
            logger.LogInformation("Ticket with id : {TicketId} cached in Redis memmory successfully", context.Message.TicketId);
        }
    }
}
