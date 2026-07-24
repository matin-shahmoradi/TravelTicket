using BuildingBlocks.Infrastracture.Outbox;
using Microsoft.EntityFrameworkCore;
namespace Ordering.Application.Data
{
    public interface IOrderDbContext
    {
        DbSet<Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }
        DbSet<Customer> Customers { get; }
        DbSet<OutboxMessage> OutboxMessages { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    }
}
