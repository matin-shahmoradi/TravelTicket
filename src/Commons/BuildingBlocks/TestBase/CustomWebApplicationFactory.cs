using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using System.Data.Common;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Xunit;

namespace BuildingBlocks.TestBase
{
    public class CustomWebApplicationFactory<TEntryPoint, TDbContext> :
        WebApplicationFactory<TEntryPoint>, IAsyncLifetime
        where TEntryPoint : class
        where TDbContext : DbContext
    {
        private readonly PostgreSqlContainer _dbContainer =
            new PostgreSqlBuilder(image: "postgres:latest")
            .WithDatabase("test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        private readonly RabbitMqContainer _rabbitMqContainer =
            new RabbitMqBuilder("rabbitmq:3-management")
            .WithPortBinding(8080, true)
            .WithUsername("guest")
            .WithPassword("guest")
            .Build();
        private DbConnection _dbConnection = null!;
        private Respawner _respawner = null!;

        public IServiceProvider serviceProvider => this.Services;

        public HttpClient HttpClient { get; private set; } = null!;
        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            await ApplyMigrationAsync();

            _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
            HttpClient = CreateClient();

            await _dbConnection.OpenAsync();
            await InitializeRespawnerAsync();
        }
        public new async Task DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
        }


        public async Task ResetDatabaseAsync()
        {
            await _respawner.ResetAsync(_dbConnection);
        }

        public async Task ApplyMigrationAsync()
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TDbContext>();

            await context.Database.MigrateAsync();
        }

        public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            using var scope = serviceProvider.CreateScope();

            var result = await action(scope.ServiceProvider);

            return result;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("test");
            Environment.SetEnvironmentVariable("ConnectionStrings:CatalogDefaultConnection", _dbContainer.GetConnectionString());
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });
            builder.ConfigureServices(services =>
            {
                services.Remove(services.SingleOrDefault(service => typeof(DbContextOptions<TDbContext>) == service.ServiceType)!);
                services.Remove(services.SingleOrDefault(service => typeof(DbConnection) == service.ServiceType)!);

                services.AddDbContext<TDbContext>(
                    options => options.UseNpgsql(_dbContainer.GetConnectionString())
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors());

                services.AddAuthentication(defaults =>
                {
                    defaults.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    defaults.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, options => { });
            });


        }
        private async Task InitializeRespawnerAsync()
        {
            _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres
            });
        }
    }

    [CollectionDefinition("IntegrationTest")]
    public class SharedTestCollection<TEntryPoint, TDbContext> :
        ICollectionFixture<CustomWebApplicationFactory<TEntryPoint, TDbContext>>
        where TEntryPoint : class
        where TDbContext : DbContext;
}
