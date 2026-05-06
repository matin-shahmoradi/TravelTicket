using Basket.API.Basket.UpdateItemPriceInBasket;
using BuildingBlocks.Messaging.Events.CatalogEvents;
using MassTransit;

namespace Basket.API.EventHandlers.CatalogEvents
{
    public class TicketPriceChangedEventConsumer(
        ISender sender,
        ILogger<TicketPriceChangedEventConsumer> logger) 
        : IConsumer<TicketPriceChangedIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<TicketPriceChangedIntegrationEvent> context)
        {
            logger.LogInformation("Integration Event Handled: {IntegrationEvent}", context.Message.GetType().Name);

            var command = new UpdateItemPriceBasketCommand(
                    context.Message.TicketId,
                    context.Message.Price);

            var sendToHandler = await sender.Send(command);
            if (!sendToHandler.IsSuccess)
            {
                logger.LogError("Error updating price in basket for id : {TicketId}" , context.Message.TicketId);
            }

            logger.LogInformation("Price for ticket id : {TicketId} updated in basket", context.Message.TicketId);
        }
    }
}
