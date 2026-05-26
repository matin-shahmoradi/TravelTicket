using AuthService.Auth.Login;
using AuthService.Auth.Register;
using AuthService.Data;
using AuthService.Interfaces;
using AuthService.Model;
using AuthService.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace AuthService
{
    public static class AuthExtensions
    {
        public static IServiceCollection AuthServices(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddControllers();

            services.AddOpenApi();

            services.AddDbContext<AuthDbContext>(cfg =>
            {
                cfg.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(cfg =>
            {
                cfg.Password.RequiredLength = 8;
            })
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();

            // DI Configurations
            services.AddScoped<IUserQueryService , UserQueryService>();
            services.AddScoped<RegisterService>();
            services.AddScoped<LoginService>();

            services.AddSwaggerGen();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
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

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                    };
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly",policy => policy.RequireRole(Roles.SuperAdmin));
            });
            return services;
        }
    }
}
