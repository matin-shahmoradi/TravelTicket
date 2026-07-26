using Basket.API.Data.Repositories;
using Basket.API.Events;
using BuildingBlocks.Abstractions;
using BuildingBlocks.Infrastracture.Outbox.Extensions;
using BuildingBlocks.Logger;
using BuildingBlocks.Messaging.Events.BasketEvents;

namespace Basket.API.EventHandlers.BasketEvents
{
    public class BasketCheckOutEventHandler(
        IBasketDbContext context,
        ICurrentUser currentUser,
        ILogger<BasketCheckOutEventHandler> logger)
        : INotificationHandler<BasketCheckOutEvent>
    {
        public async Task Handle(BasketCheckOutEvent notification, CancellationToken cancellationToken)
        {
            //TODO : add customer phone number in auth service
            var BasketCheckoutedIntegrationEvent = new BasketCheckOutIntegrationEvent
            {
                CustomerId = currentUser.UserId,
                Email = currentUser.UserEmail,
                PhoneNumber = currentUser.PhoneNumber,
                Items = notification.Items.Select(x => new BasketCheckOutIntegrationEventItem
                (
                    TicketId: x.TicketId,
                    Price: x.Price,
                    Quantity: x.Quantity
                )).ToList()
            };
            try
            {
                context.outboxMessages.AddIntegrationEvent(BasketCheckoutedIntegrationEvent);
                await context.SaveChangesAsync(cancellationToken);
                logger.LogEventHandler(notification.GetType().Name);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to handle {event}", notification.GetType().Name);
            }

        }
    }
}
