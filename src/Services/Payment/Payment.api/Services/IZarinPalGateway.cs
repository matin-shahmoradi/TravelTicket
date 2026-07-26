using Payment.api.Domain.DTOs;

namespace Payment.api.Services
{
    public interface IZarinPalGateway
    {
        Task<ZarinPalRequestResult> RequestPaymentAsync(
        decimal amount,
        string description,
        string callbackUrl,
        CancellationToken cancellationToken);

        Task<ZarinPalVerifyResult> VerifyPaymentAsync(
            decimal amount,
            string authority,
            CancellationToken cancellationToken);
    }
}
