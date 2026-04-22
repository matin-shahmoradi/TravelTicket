using Microsoft.EntityFrameworkCore;
namespace Ordering.Application.Data
{
    public interface IOrderDbContext
    {
        DbSet<Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }
        DbSet<Customer> Customers { get; }
        DbSet<Ticket> Tickets { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    }
}
