using Basket.API.Events;
using BuildingBlocks.Abstractions;
using BuildingBlocks.Messaging.Events.BasketEvents;
using MassTransit;

namespace Basket.API.EventHandlers.BasketEvents
{
    public class BasketCheckOutEventHandler(
        IBus bus,
        ICurrentUser currentUser,
        ILogger<BasketCheckOutEventHandler> logger)
        : INotificationHandler<BasketCheckOutEvent>
    {
        public Task Handle(BasketCheckOutEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("[EVENT] Event handled : {Event}", notification.GetType().Name);
            //TODO : add customer phone number in auth service
            var BasketCheckoutedIntegrationEvent = new BasketCheckOutIntegrationEvent
            {
                CustomerId = currentUser.UserId!,
                Email = currentUser.UserEmail!,
                PhoneNumber = null!,
                Items = notification.Items.Select(x => new BasketCheckOutIntegrationEventItem
                (
                    TicketId: x.TicketId,
                    Price: x.Price,
                    Quantity: x.Quantity
                )).ToList()
            };

            return bus.Publish(BasketCheckoutedIntegrationEvent);
        }
    }
}
