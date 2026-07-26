using BuildingBlocks;
using BuildingBlocks.CQRS;
using FluentValidation;
using Payment.api.Domain.DTOs;

namespace Payment.api.Features.GetPaymentById
{
    public sealed record GetPaymentUrlQuery(Guid OrderId) : IQuery<Result<ZarinPalRequestResult>>;

    public class GetPaymentByIdQueryValidator : AbstractValidator<GetPaymentUrlQuery>
    {
        public GetPaymentByIdQueryValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Payment id cant be empty");
        }
    }
}
