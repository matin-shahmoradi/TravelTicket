using Payment.api.Domain;

namespace Payment.api.Repositories
{
    public interface IPaymentRepository
    {
        Task<PaymentModel?> GetByIdAsync(
            Guid paymentId,
            CancellationToken cancellationToken = default);
        Task<PaymentModel?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<PaymentModel?> GetByAuthorityAsync(string authority, CancellationToken cancellationToken);
        Task AddAsync(PaymentModel payment, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
