using BuildingBlocks.DDD;
using Payment.api.Domain.Enums;
using Payment.api.Domain.Events;
using Payment.api.Domain.Exceptions;
using Payment.api.Domain.StrongIdTypes;

namespace Payment.api.Domain
{
    public sealed class PaymentModel : Aggregate<PaymentId>
    {
        public OrderId OrderId { get; private set; }
        public CustomerId CustomerId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentStatus Status { get; private set; } = PaymentStatus.None;

        public string? Authority { get; private set; }
        public string? TransactionId { get; private set; }
        public string? FailureReason { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? PaidAtUtc { get; private set; }

        private PaymentModel()
        {
            // for ef core
        }

        private PaymentModel(
            PaymentId id,
            OrderId orderId,
            CustomerId customerId,
            decimal amount,
            DateTime createdAtUtc)
        {
            Id = id;
            OrderId = orderId;
            CustomerId = customerId;
            Amount = amount;
            Status = PaymentStatus.Pending;
            CreatedAtUtc = createdAtUtc;
        }
        public static PaymentModel Create(Guid orderId, Guid customerId, decimal amount, DateTime createdAtUtc)
        {
            if (orderId == Guid.Empty)
                throw new DomainException("Order is required.");
            if (customerId == Guid.Empty)
                throw new DomainException("Order is required.");

            return new PaymentModel
            {
                Id = PaymentId.New(),
                OrderId = OrderId.Of(orderId),
                CustomerId = CustomerId.Of(customerId),
                Amount = amount,
                CreatedAtUtc = createdAtUtc
            };
        }

        public void MarkGatewayRequested(string authority)
        {
            if (Status != PaymentStatus.Pending)
                throw new DomainException("Only pending payments can be sent to gateway.");

            if (string.IsNullOrWhiteSpace(authority))
                throw new DomainException("Authority is required.");

            Authority = authority;
            Status = PaymentStatus.GatewayRequested;
        }

        public void MarkSucceeded(
           string authority,
           string transactionId,
           DateTime paidAtUtc)
        {
            if (Status == PaymentStatus.Succeeded)
                return;

            if (Status != PaymentStatus.GatewayRequested && Status != PaymentStatus.Pending)
                throw new DomainException("Payment cannot be marked as succeeded.");

            Authority = authority;
            TransactionId = transactionId;
            Status = PaymentStatus.Succeeded;
            PaidAtUtc = paidAtUtc;
            FailureReason = null;

            AddDomainEvents(new PaymentSucceededDomainEvent(
               PaymentId: Id.Value,
               OrderId: OrderId.Value,
               CustomerId: CustomerId.Value,
               PaymentStatus: Status.ToString(),
               Amount: Amount,
               Authority: authority,
               TransactionId: transactionId,
               PaidAtUtc: paidAtUtc,
               OccurredOnUtc: DateTime.UtcNow));
        }

        public void MarkFailed(string reason)
        {
            if (Status == PaymentStatus.Succeeded)
                throw new DomainException("Succeeded payment cannot be failed.");

            Status = PaymentStatus.Failed;
            FailureReason = reason;
        }
    }
}
