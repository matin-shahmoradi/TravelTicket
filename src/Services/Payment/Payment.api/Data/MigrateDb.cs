using Microsoft.EntityFrameworkCore;

namespace Payment.api.Data
{
    public static class MigrateDb
    {
        public async static Task MigrateDatabase(this WebApplication app)
        {
            using var scoped = app.Services.CreateScope();

            var context = scoped.ServiceProvider.GetRequiredService<PaymentDbContext>();

            await context.Database.MigrateAsync();
        }
    }
}
