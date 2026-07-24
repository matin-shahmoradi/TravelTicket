using BuildingBlocks.Messaging.Events.OrderEvents;
using MassTransit;
using MediatR;
using Payment.api.Features.PaymentRequest;

namespace Payment.api.Consumers
{
    public sealed class OrderPaymentRequestedConsumer(
        ISender sender,
        ILogger<OrderPaymentRequestedConsumer> logger)
        : IConsumer<OrderCreatedIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
        {
            var command = new CreatePaymentCommand(
                OrderId: context.Message.OrderId,
                CustomerId: context.Message.CustomerId,
                Amount: context.Message.Amount);

            try
            {
                var send = await sender.Send(command);
                logger.LogInformation(
                    "[PaymentRequestCommandHandler] consumed {context}",
                     context.Message.GetType().Name);
            }
            catch (Exception ex)
            {
                logger.LogError(
                   exception: ex,
                   message: "[Handler] Failed to handle {command}",
                    command.GetType().Name);
            }
        }
    }
}
