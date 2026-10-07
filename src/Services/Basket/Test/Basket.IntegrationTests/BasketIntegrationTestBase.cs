using Basket.API.Data;
using BuildingBlocks.Infrastracture.Outbox;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Basket.IntegrationTests
{
    public class BasketIntegrationTestBase : IClassFixture<BasketIntegrationTestFactory>, IAsyncLifetime
    {
        protected readonly BasketIntegrationTestFactory _factory;
        protected readonly IDistributedCache _cache;
        protected readonly BasketDbContext _backetDbContext;
        protected readonly IOutboxProcessor _outboxProcessor;
        protected static readonly Guid TestUserId = Guid.NewGuid();
        public BasketIntegrationTestBase(BasketIntegrationTestFactory factory)
        {
            _factory = factory;
            _factory.FakeCurrentUser.UserId = TestUserId;
            var scope = _factory.serviceProvider.CreateScope();
            _cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
            _backetDbContext = scope.ServiceProvider.GetRequiredService<BasketDbContext>();
            _outboxProcessor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();
        }

        public async Task InitializeAsync()
        {
            await _factory.ResetDatabaseAsync();
            await _factory.ApplyMigrationAsync();
            _factory.CatalogGrpcClientMock.Reset();
        }
        public Task DisposeAsync() => Task.CompletedTask;

    }
}
