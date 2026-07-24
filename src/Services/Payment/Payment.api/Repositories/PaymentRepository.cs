using Microsoft.EntityFrameworkCore;
using Payment.api.Data;
using Payment.api.Domain;
using Payment.api.Domain.StrongIdTypes;

namespace Payment.api.Repositories
{
    internal sealed class PaymentRepository(PaymentDbContext dbContext) : IPaymentRepository
    {
        public async Task<PaymentModel?> GetByIdAsync(Guid paymentId, CancellationToken cancellationToken)
        {
            return await dbContext.Payments.FindAsync(PaymentId.Of(paymentId), cancellationToken);
        }

        public async Task<PaymentModel?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
        {
            return await dbContext.Payments.SingleOrDefaultAsync(x => x.OrderId == OrderId.Of(orderId));
            //return dbContext.Payments
            //    .FirstOrDefaultAsync(payment => payment.OrderId == OrderId.Of(orderId), cancellationToken);
        }

        public Task<PaymentModel?> GetByAuthorityAsync(string authority, CancellationToken cancellationToken)
        {
            return dbContext.Payments
                .FirstOrDefaultAsync(payment => payment.Authority == authority, cancellationToken);
        }

        public async Task AddAsync(PaymentModel payment, CancellationToken cancellationToken) =>
            await dbContext.Payments.AddAsync(payment, cancellationToken);
        public async Task SaveChangesAsync(CancellationToken cancellationToken) =>
            await dbContext.SaveChangesAsync(cancellationToken);
    }
}
