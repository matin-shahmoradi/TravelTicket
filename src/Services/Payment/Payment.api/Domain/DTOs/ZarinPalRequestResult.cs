using System.Text.Json;
using System.Text.Json.Serialization;

namespace Payment.api.Domain.DTOs
{
    public sealed record ZarinPalRequestResult(
        bool IsSuccess,
        string? Authority,
        string? PaymentUrl,
        int? Status,
        string? ErrorMessage);

    public sealed class ZarinpalRequestData
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("authority")]
        public string? Authority { get; set; }

        [JsonPropertyName("fee_type")]
        public string? FeeType { get; set; }

        [JsonPropertyName("fee")]
        public int Fee { get; set; }
    }

    public sealed class ZarinpalRequestResponse
    {
        [JsonPropertyName("data")]
        public ZarinpalRequestData? Data { get; set; }

        [JsonPropertyName("errors")]
        public JsonElement Errors { get; set; }
    }

    public sealed record ZarinPalVerifyResult(
        bool IsSuccess,
        string? TransactionId,
        string? ErrorMessage);

    public sealed class ZarinpalVerifyData
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("card_hash")]
        public string? CardHash { get; set; }

        [JsonPropertyName("card_pan")]
        public string? CardPan { get; set; }

        [JsonPropertyName("ref_id")]
        public long RefId { get; set; }

        [JsonPropertyName("fee_type")]
        public string? FeeType { get; set; }

        [JsonPropertyName("fee")]
        public int Fee { get; set; }
    }

    public sealed class ZarinpalVerifyResponse
    {
        [JsonPropertyName("data")]
        public ZarinpalVerifyData? Data { get; set; }

        [JsonPropertyName("errors")]
        public JsonElement Errors { get; set; }
    }

}
