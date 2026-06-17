using Basket.API.Data.Repositories;
using Basket.API.Grpc;
using Basket.API.Grpc.GrpcClients;
using Basket.API.Services;
using BuildingBlocks.Abstractions;
using BuildingBlocks.EntityFramwork.Interceptors;
using BuildingBlocks.Messaging.Events.Extensions;
using Catalog.Grpc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Basket.API.BasketExtensions
{
    public static class BasketExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });
            services.AddDbContext<BacketDbContext>((sp, cfg) =>
            {
                cfg.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                cfg.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddStackExchangeRedisCache(config =>
            {
                config.Configuration = configuration.GetConnectionString("Redis");
                config.InstanceName = "BasketRedis";
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
                AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidateIssuer = true,

                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"],

                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),

                        RoleClaimType = ClaimTypes.Role
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("User", policy =>
                {
                    policy.RequireRole("User");
                });
            });

            services.AddHttpContextAccessor();
            services.AddMassTransitWithAssembly(configuration, assembly);


            services.AddGrpcClient<CatalogGrpcService.CatalogGrpcServiceClient>(option =>
            {
                option.Address = new Uri(configuration["GrpcServices:Catalog"]!);
            });

            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<ICacheTicketRepository, CacheTicketRepository>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<ICatalogGrpcClient, CatalogGrpcClient>();
            services.AddScoped<ISaveChangesInterceptor, AuditInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventInterceptor>();
            services.Decorate<IBasketRepository, CachedBasketRepository>();

            services.AddCarter();
            services.AddValidatorsFromAssembly(assembly);
            services.AddSwaggerGen();
            services.AddProblemDetails();

            services.AddHealthChecks()
                .AddNpgSql(
                    connectionString: configuration.GetConnectionString("DefaultConnection")!,
                    name: "postgres",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "ready" }
                    )
                .AddRedis(
                    redisConnectionString: configuration.GetConnectionString("Redis")!,
                    name: "redis",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "ready" });

            return services;
        }

        public static async Task MigrateDatabase(this WebApplication app)
        {
            using var scoped = app.Services.CreateScope();
            var context = scoped.ServiceProvider.GetRequiredService<BacketDbContext>();

            await context.Database.MigrateAsync();
        }
    }
}
