using System.ComponentModel.DataAnnotations.Schema;

namespace Ordering.Domain.ValueObjects
{
    [ComplexType]
    public record Payment
    {
        public string? CardName { get; } = default!;
        public string CardNumber { get; } = default!;
        public string Expiration { get;} = default!;
        public string CVV { get; } = default!;
        public int PaymentMethod { get;} = default!;

        protected Payment() { }

        private Payment(string cardName, string cardNumber, string expiration , string cvv, int paymentMethod)
        {
            CardName = cardName;
            CardNumber = cardNumber;
            Expiration = expiration;
            CVV = cvv;
            PaymentMethod = paymentMethod;
        }

        public static Payment New(string cardName, string cardNumber, string expiration, string cvv, int paymentMethod)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(cardName);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(cardNumber);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(expiration);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(cvv);

            return new Payment(cardName,cardNumber,expiration,cvv,paymentMethod);
        }
    }
}
