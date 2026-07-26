using BuildingBlocks;
using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Microsoft.Extensions.Options;
using Payment.api.Domain;
using Payment.api.Options;
using Payment.api.Repositories;
using Payment.api.Services;

namespace Payment.api.Features.PaymentRequest
{
    internal sealed class CreatePaymentCommandHandler(
        ITransactionExecutor transaction,
        IPaymentRepository paymentRepository,
        IZarinPalGateway zarinPalGateway,
        IOptions<ZarinPalOptions> options,
        ILogger<CreatePaymentCommandHandler> logger
        )
        : ICommandHandler<CreatePaymentCommand, Result<RequestPaymentResult>>
    {
        public async Task<Result<RequestPaymentResult>> Handle(CreatePaymentCommand command, CancellationToken cancellationToken)
        {
            var payment = PaymentModel.Create(
                orderId: command.OrderId,
                customerId: command.CustomerId,
                amount: command.Amount,
                createdAtUtc: DateTime.UtcNow);

            var callbackUrl = $"{options.Value.CallbackUrl}?paymentId={payment.Id.Value}";

            var gatewayResult = await zarinPalGateway.RequestPaymentAsync(
                amount: command.Amount,
                description: $"Payment for OrderId : {command.OrderId}",
                callbackUrl: callbackUrl,
                cancellationToken: cancellationToken
                );


            return await transaction.ExecuteAsync(async ct =>
            {
                if (!gatewayResult.IsSuccess ||
                    gatewayResult.Authority is null ||
                    gatewayResult.PaymentUrl is null)
                {
                    payment.MarkFailed(gatewayResult.ErrorMessage ?? "Gateway request failed.");
                    try
                    {
                        await paymentRepository.AddAsync(payment, cancellationToken);
                        return Result<RequestPaymentResult>.Failure(Error.UnprocessableEntity("Payment gateway request Failed."));
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(
                            ex,
                            "Failed to mark payment status payment request. Payment id : {id}",
                            payment.Id
                            );
                        return Result<RequestPaymentResult>.Failure(
                            Error.Internal_Server(message: ex.Message));
                    }
                }

                if (gatewayResult.IsSuccess)
                    payment.MarkPending();

                payment.MarkGatewayRequested(gatewayResult.Authority);

                try
                {
                    await paymentRepository.AddAsync(payment, cancellationToken);

                    var result = new RequestPaymentResult(
                        PaymentId: payment.Id.Value,
                        Authority: gatewayResult.Authority,
                        PaymentUrl: gatewayResult.PaymentUrl,
                        Status: gatewayResult.Status!.Value);

                    return Result<RequestPaymentResult>.Success(result);
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Failed to mark payment status payment request. Payment id : {id}",
                        payment.Id
                        );
                    return Result<RequestPaymentResult>.Failure(
                        Error.Internal_Server(message: ex.Message));
                }
            });
        }
    }
}
