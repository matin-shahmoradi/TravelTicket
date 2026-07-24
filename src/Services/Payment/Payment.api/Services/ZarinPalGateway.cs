using Microsoft.Extensions.Options;
using Payment.api.Domain.DTOs;
using Payment.api.Options;

namespace Payment.api.Services
{
    public sealed class ZarinPalGateway(IHttpClientFactory httpClient, IOptions<ZarinPalOptions> options) : IZarinPalGateway
    {
        public async Task<ZarinPalRequestResult> RequestPaymentAsync(
            decimal amount,
            string description,
            string callbackUrl,
            CancellationToken cancellationToken)
        {
            var request = new
            {
                merchant_id = options.Value.MerchantId,
                amount = amount,
                callback_url = callbackUrl,
                description = description,
            };

            var response = await httpClient
                .CreateClient()
                .PostAsJsonAsync(
                    options.Value.RequestUrl,
                    request,
                    cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new ZarinPalRequestResult(
                   IsSuccess: false,
                   Authority: null,
                   PaymentUrl: null,
                   Status: null,
                   ErrorMessage: "Zarinpal request failed."
                   );
            }

            var result = await response.Content.ReadFromJsonAsync<ZarinpalRequestResponse>(cancellationToken: cancellationToken);

            if (result?.Data?.Code is not 100)
            {
                return new ZarinPalRequestResult(
                   IsSuccess: false,
                   Authority: null,
                   PaymentUrl: null,
                   Status: result?.Data?.Code,
                   ErrorMessage: result?.Errors.ToString());
            }

            var paymentUrl = $"{options.Value.PaymentPageUrl}{result.Data.Authority}";
            return new ZarinPalRequestResult(
                   IsSuccess: true,
                   Authority: result.Data.Authority,
                   PaymentUrl: paymentUrl,
                   Status: result?.Data?.Code,
                   ErrorMessage: null
                   );
        }

        public async Task<ZarinPalVerifyResult> VerifyPaymentAsync(
            decimal amount,
            string authority,
            CancellationToken cancellationToken)
        {
            var request = new
            {
                merchant_id = options.Value.MerchantId,
                amount = amount,
                authority = authority
            };

            var response = await httpClient
                .CreateClient()
                .PostAsJsonAsync(
                    requestUri: options.Value.VerifyUrl,
                    request,
                    cancellationToken: cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new ZarinPalVerifyResult(
                    IsSuccess: false,
                    TransactionId: null,
                    ErrorMessage: "Zarinpal verify failed.");
            }

            var result = await response.Content.ReadFromJsonAsync<ZarinpalVerifyResponse>(
            cancellationToken: cancellationToken);

            if (result?.Data?.Code is not 100 and not 101)
            {
                return new ZarinPalVerifyResult(
                   IsSuccess: false,
                   TransactionId: null,
                   ErrorMessage: result?.Errors.ToString());
            }

            return new ZarinPalVerifyResult(
               IsSuccess: true,
               TransactionId: result.Data.RefId.ToString(),
               ErrorMessage: null);
        }
    }
}
