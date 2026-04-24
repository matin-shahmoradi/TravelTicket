using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Infrastructure.Data;

namespace Ordering.Infrastructure.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

            await context.Database.MigrateAsync();
            await SeedAsync(context);
        }

        private static async Task SeedAsync(OrderDbContext orderDbContext)
        {
            using var transaction = await orderDbContext.Database.BeginTransactionAsync();
            try
            {
                await SeedCustomerAsync(orderDbContext);
                await SeedTicketAsync(orderDbContext);
                await SeedOrdersWithItemsAsync(orderDbContext);

                await orderDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"seeing operation failed : {ex}", ex);
                }
            }
        }

        private static async Task SeedCustomerAsync(OrderDbContext orderDbContext)
        {
            if (!await orderDbContext.Customers.AnyAsync())
            {
                await orderDbContext.Customers.AddRangeAsync(InitialData.Customers);
            }
        }

        private static async Task SeedTicketAsync(OrderDbContext orderDbContext)
        {
            if (!await orderDbContext.Tickets.AnyAsync())
            {
                await orderDbContext.Tickets.AddRangeAsync(InitialData.Tickets);
            }
        }
        private static async Task SeedOrdersWithItemsAsync(OrderDbContext orderDbContext)
        {
            if (!await orderDbContext.Orders.AnyAsync())
            {
                await orderDbContext.Orders.AddRangeAsync(InitialData.OrdersWithItems);
            }
        }

    }
}
