using MediatR;
using Payment.api.Domain.Events;
using Payment.api.Features.VerifyPayment;

namespace Payment.api.EventHandlers
{
    public class PaymentRequestEventHandler(
        ISender sender,
        ILogger<PaymentRequestEventHandler> logger) : INotificationHandler<PaymentRequestedEvent>
    {
        public async Task Handle(PaymentRequestedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await sender.Send(new VerifyPaymentCommand(
                   PaymentId: notification.PaymentId,
                   Authority: notification.Authority,
                   Status: notification.Status));
                logger.LogInformation("[EVENT HANDLER] event {event} handled", notification.GetType().Name);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "[EVENT HANDLER] Failed to handle {event} event",
                    notification.GetType().Name);
                throw;
            }
        }
    }
}
