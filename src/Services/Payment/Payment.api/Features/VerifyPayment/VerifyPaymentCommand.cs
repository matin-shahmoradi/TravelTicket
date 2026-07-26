using BuildingBlocks;
using BuildingBlocks.CQRS;
using FluentValidation;

namespace Payment.api.Features.VerifyPayment
{
    public sealed record VerifyPaymentResult(
        bool IsSuccess,
        Guid PaymentId,
        string? TransactionId);
    public sealed record VerifyPaymentCommand(
        Guid PaymentId,
        string Authority,
        string Status) : ICommand<Result<VerifyPaymentResult>>;

    public class VerifyPaymentCommandValidator : AbstractValidator<VerifyPaymentCommand>
    {
        public VerifyPaymentCommandValidator()
        {
            RuleFor(x => x.PaymentId)
                .NotEmpty().WithMessage("Payment Id is required");

            RuleFor(x => x.Authority)
                .NotEmpty().WithMessage("Authority is required");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required");
        }
    }
}
