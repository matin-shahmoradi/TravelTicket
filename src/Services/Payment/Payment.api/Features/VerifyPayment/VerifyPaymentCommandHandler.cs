using BuildingBlocks;
using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Payment.api.Domain.DTOs;
using Payment.api.Domain.Enums;
using Payment.api.Repositories;
using Payment.api.Services;

namespace Payment.api.Features.VerifyPayment
{
    internal sealed class VerifyPaymentCommandHandler(
        ITransactionExecutor transaction,
        IPaymentRepository paymentRepository,
        IZarinPalGateway zarinPalGateway,
        ILogger<VerifyPaymentCommandHandler> logger)
        : ICommandHandler<VerifyPaymentCommand, Result<VerifyPaymentResult>>
    {
        public async Task<Result<VerifyPaymentResult>> Handle(VerifyPaymentCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var payment = await paymentRepository.GetByIdAsync(command.PaymentId, cancellationToken);

                if (payment is null)
                    return Result<VerifyPaymentResult>.Failure(
                        Error.NotFoundError(message: "Payment not found"));

                if (payment.Status == PaymentStatus.Succeeded)
                {
                    var result = new VerifyPaymentResult(
                       IsSuccess: true,
                       PaymentId: payment.Id.Value,
                       TransactionId: payment.TransactionId);
                    return Result<VerifyPaymentResult>.Success(result);
                }

                var userCancelled = !string.Equals(command.Status, "OK", StringComparison.OrdinalIgnoreCase);

                ZarinPalVerifyResult? verifyResult = null;
                if (!userCancelled)
                {
                    verifyResult = await zarinPalGateway.VerifyPaymentAsync(
                        amount: payment.Amount,
                        authority: command.Authority,
                        cancellationToken: cancellationToken);
                }

                return await transaction.ExecuteAsync(async ct =>
                {
                    var currentPayment = await paymentRepository.GetByIdAsync(command.PaymentId, ct);

                    if (currentPayment is null)
                    {
                        return Result<VerifyPaymentResult>.Failure(
                        Error.NotFoundError(message: "Payment not found"));
                    }

                    if (currentPayment.Status == PaymentStatus.Succeeded)
                    {
                        return Result<VerifyPaymentResult>.Success(
                        new VerifyPaymentResult(
                            IsSuccess: true,
                            PaymentId: currentPayment.Id.Value,
                            TransactionId: currentPayment.TransactionId));
                    }
                    if (userCancelled)
                    {
                        currentPayment.MarkFailed("User cancelled payment");
                        return Result<VerifyPaymentResult>.Failure(
                            Error.UnprocessableEntity(message: "User cancelled payment."));
                    }

                    if (verifyResult is null ||
                        !verifyResult.IsSuccess ||
                        string.IsNullOrWhiteSpace(verifyResult.TransactionId))
                    {
                        currentPayment.MarkFailed(
                        verifyResult?.ErrorMessage ?? "Payment verification failed.");

                        return Result<VerifyPaymentResult>.Failure(
                            Error.UnprocessableEntity(message: "Payment verification failed"));
                    }

                    currentPayment.MarkSucceeded(
                        authority: command.Authority,
                        transactionId: verifyResult.TransactionId,
                        paidAtUtc: DateTime.UtcNow);

                    return Result<VerifyPaymentResult>.Success(
                        new VerifyPaymentResult(
                            IsSuccess: true,
                            PaymentId: currentPayment.Id.Value,
                            TransactionId: verifyResult.TransactionId));
                }, cancellationToken);

            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error while verifying payment. PaymentId: {PaymentId}, Authority: {Authority}",
                    command.PaymentId,
                    command.Authority);

                return Result<VerifyPaymentResult>.Failure(
                    Error.Internal_Server(message: "An unexpected error occurred while verifying payment."));
            }
        }
    }
}
