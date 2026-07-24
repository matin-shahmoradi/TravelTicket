using BuildingBlocks;
using BuildingBlocks.CQRS;
using Microsoft.Extensions.Options;
using Payment.api.Domain.DTOs;
using Payment.api.Options;
using Payment.api.Repositories;

namespace Payment.api.Features.GetPaymentById
{
    internal sealed class GetPaymentUrlQueryHandler(
        IPaymentRepository paymentRepository,
        IOptions<ZarinPalOptions> zarinPalOption)
        : IQueryHandler<GetPaymentUrlQuery, Result<ZarinPalRequestResult>>
    {
        public async Task<Result<ZarinPalRequestResult>> Handle(GetPaymentUrlQuery query, CancellationToken cancellationToken)
        {
            var payment = await paymentRepository.GetByOrderIdAsync(query.OrderId);

            if (payment is null)
                return Result<ZarinPalRequestResult>.Failure(Error.NotFoundError($"Payment with id {query.OrderId} not found."));

            string payment_url = $"{zarinPalOption.Value.PaymentPageUrl}{payment?.Authority}";

            var result = new ZarinPalRequestResult(true, payment!.Authority, payment_url, 100, null);
            return Result<ZarinPalRequestResult>.Success(result);
        }
    }
}
