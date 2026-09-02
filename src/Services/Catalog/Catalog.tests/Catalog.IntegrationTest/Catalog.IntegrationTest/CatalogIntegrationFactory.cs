using BuildingBlocks.TestBase;
using Catalog.API.Data;
using Catalog.IntegrationTest.FakeData;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.IntegrationTest
{
    public sealed class CatalogIntegrationFactory : CustomWebApplicationFactory<Program, CatalogDbContext>
    {
        protected override async Task SeedDatabaseAsync()
        {
            await SeedTicketsAsync(20);
        }

        public async Task SeedTicketsAsync(int count, bool useNewSeed = false)
        {
            await using var scope = Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

            var seeder = new FakeTicket(dbContext);

            await seeder.SeedTicket(count, useNewSeed);
        }

    }
}
