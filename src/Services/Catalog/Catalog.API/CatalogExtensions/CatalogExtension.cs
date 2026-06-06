using BuildingBlocks.Abstractions;
using BuildingBlocks.EntityFramwork.Interceptors;
using BuildingBlocks.Messaging.Events.Extensions;
using Catalog.API.EventHandlers;
using Catalog.API.Tickets.CurrentUser;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Catalog.API.CatalogExtensions
{
    public static class CatalogExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var jwtSetting = configuration.GetSection("JwtSetting");

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<TicketCreatedEventHandler>();
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSetting["Issuer"],
                        ValidAudience = jwtSetting["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSetting["Key"]!)),

                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.Name,
                    };
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                {
                    policy.RequireRole("Admin", "SuperAdmin");
                });
            });

            // Fluent Validation configuration.
            services.AddValidatorsFromAssembly(assembly);

            //DI configurations
            services.AddScoped<ICatalogDbContext, CatalogDbContext>();
            services.AddScoped<ISaveChangesInterceptor, AuditInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventInterceptor>();
            services.AddScoped<ICurrentUser, CurrentUser>();

            services.AddDbContext<CatalogDbContext>((sp, cfg) =>
            {
                cfg.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                cfg.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddMassTransitWithAssembly(configuration, Assembly.GetExecutingAssembly());

            services.AddCarter();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHttpContextAccessor();
            services.AddProblemDetails();
            return services;
        }
    }
}
