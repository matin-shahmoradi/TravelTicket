using BuildingBlocks;
using BuildingBlocks.CQRS;
using FluentValidation;

namespace Payment.api.Features.PaymentRequest
{
    public sealed record RequestPaymentResult(Guid PaymentId, string Authority, string PaymentUrl, int Status);
    public sealed record CreatePaymentCommand(
         Guid OrderId,
         Guid CustomerId,
         decimal Amount) : ICommand<Result<RequestPaymentResult>>;

    public class PaymentRequestCommandValidator : AbstractValidator<CreatePaymentCommand>
    {
        public PaymentRequestCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Order is required");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Customer id is required");

            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("Amount is required")
                .GreaterThanOrEqualTo(0).WithMessage("Amount cant be negative number");
        }
    }

}
