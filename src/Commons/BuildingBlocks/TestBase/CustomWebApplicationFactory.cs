using BuildingBlocks.Infrastracture.Outbox;
using MassTransit;
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
            .WithPortBinding(5672, true)
            .WithPortBinding(15672, true)
            .WithUsername("guest")
            .WithPassword("guest")
            .Build();
        private DbConnection _dbConnection = null!;
        private Respawner _respawner = null!;
        public IServiceProvider serviceProvider => this.Services;
        public HttpClient HttpClient { get; private set; } = null!;

        public virtual async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            await _rabbitMqContainer.StartAsync();

            await ApplyMigrationAsync();

            _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
            HttpClient = CreateClient();

            await _dbConnection.OpenAsync();
            await InitializeRespawnerAsync();

            await SeedDatabaseAsync();
        }
        public virtual new async Task DisposeAsync()
        {
            await ResetDatabaseAsync();
            await _dbContainer.DisposeAsync();
            await _dbConnection.CloseAsync();
            await _rabbitMqContainer.DisposeAsync();
        }

        public async Task ResetDatabaseAsync()
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
            await context.Database.EnsureDeletedAsync();
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

            return await action(scope.ServiceProvider);
        }
        public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using var scope = serviceProvider.CreateScope();

            await action(scope.ServiceProvider);
        }

        public async Task<T> ExecuteDbContextAsync<T>(Func<TDbContext, Task<T>> action)
        {
            return await ExecuteScopeAsync(sp =>
            {
                var context = sp.GetRequiredService<TDbContext>();
                return action(context);
            });
        }
        public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            return await ExecuteScopeAsync(async sp =>
            {
                var mediator = sp.GetRequiredService<IMediator>();

                TResponse response = default!;

                response = await mediator.Send(request);

                return response;
            });
        }
        public Task SendAsync(IRequest request)
        {
            return ExecuteScopeAsync(async sp =>
            {
                var mediator = sp.GetRequiredService<IMediator>();
                await mediator.Send(request);
            });
        }

        public async Task<bool> WaitUntilAsync(
            Func<Task<bool>> condition,
            TimeSpan timeout,
            TimeSpan? pollingInterval)
        {
            var interval = pollingInterval ?? TimeSpan.FromMilliseconds(200);
            var deadline = DateTime.UtcNow + timeout;

            while (DateTime.UtcNow < deadline)
            {
                if (await condition())
                {
                    return true;
                }
                await Task.Delay(interval);
            }

            return false;
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

                //ServiceRegistrationConfig.RemoveMassTransitRegistrations(services);
                //ServiceRegistrationConfig.RemoveHealthCheckRegistrations(services);


                services.AddScoped<IOutboxProcessor, OutboxProcessor<TDbContext>>();

                services.AddDbContext<TDbContext>(
                    options =>
                    {
                        options.UseNpgsql(_dbContainer.GetConnectionString())
                            .EnableSensitiveDataLogging()
                            .EnableDetailedErrors();
                    });

                services.AddMassTransitTestHarness(x =>
                {
                    x.AddConsumers(typeof(TEntryPoint).Assembly);
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(
                            host: _rabbitMqContainer.Hostname,
                            port: _rabbitMqContainer.GetMappedPublicPort(5672),
                            virtualHost: "/",
                            h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });
                        cfg.ConfigureEndpoints(context);
                    });
                });

                services.AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(typeof(TEntryPoint).Assembly);
                });

                services.AddAuthentication(defaults =>
                {
                    defaults.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    defaults.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, options => { });
            });
        }
        protected virtual Task SeedDatabaseAsync()
        {
            return Task.CompletedTask;
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
