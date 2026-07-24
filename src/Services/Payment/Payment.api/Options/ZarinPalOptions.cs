namespace Payment.api.Options
{
    public class ZarinPalOptions
    {
        public string MerchantId { get; init; } = default!;
        public string RequestUrl { get; init; } = default!;
        public string VerifyUrl { get; init; } = default!;
        public string PaymentPageUrl { get; init; } = default!;
        public string CallbackUrl { get; init; } = default!;
    }
}
