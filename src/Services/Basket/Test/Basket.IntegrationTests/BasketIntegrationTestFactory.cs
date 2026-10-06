using Basket.API.Data;
using Basket.API.Grpc;
using Basket.IntegrationTests.FakeData;
using BuildingBlocks.Abstractions;
using BuildingBlocks.TestBase;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Basket.IntegrationTests
{
    public class BasketIntegrationTestFactory : CustomWebApplicationFactory<Program, BasketDbContext>
    {
        public Mock<ICatalogGrpcClient> CatalogGrpcClientMock { get; } = new();
        public FakeCurrentUser FakeCurrentUser { get; } = new();


        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureServices(services =>
            {
                var currentUserDescriptor = services.SingleOrDefault(
                    s => s.ServiceType == typeof(ICurrentUser));
                if (currentUserDescriptor is not null)
                    services.Remove(currentUserDescriptor);

                services.AddSingleton<ICurrentUser>(FakeCurrentUser);

                var grpcDescriptor = services.SingleOrDefault(
                    s => s.ServiceType == typeof(ICatalogGrpcClient));
                if (grpcDescriptor is not null)
                    services.Remove(grpcDescriptor);

                services.AddSingleton<ICatalogGrpcClient>(CatalogGrpcClientMock.Object);

                var redisDescriptor = services.SingleOrDefault(
                      s => s.ServiceType == typeof(IDistributedCache));

                if (redisDescriptor is not null)
                    services.Remove(redisDescriptor);

                var redisOptionsDescriptor = services.SingleOrDefault(
                    s => s.ServiceType.FullName?
                        .Contains("RedisCacheOptions") == true);
                if (redisOptionsDescriptor is not null)
                    services.Remove(redisOptionsDescriptor);

                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = Environment.GetEnvironmentVariable("ConnectionStrings:Redis");
                });
            });
        }
    }
}
