using BuildingBlocks.Messaging.Events.CatalogEvents;
using MassTransit;

namespace Basket.API.EventHandlers.CatalogEvents
{
    public class TicketPriceChangedIntegrationEventHandler(
        ISender sender,
        Logger<TicketPriceChangedIntegrationEventHandler> logger) 
        :
        IConsumer<TicketPriceChangedIntegrationEvent>
    {
        public Task Consume(ConsumeContext<TicketPriceChangedIntegrationEvent> context)
        {
            logger.LogInformation("Integration Event Handled: {IntegrationEvent}", context.Message.GetType().Name);
            
            return Task.CompletedTask;
        }
    }
}
