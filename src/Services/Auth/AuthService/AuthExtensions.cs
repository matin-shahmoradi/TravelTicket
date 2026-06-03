using AuthService.Data;
using AuthService.Interfaces;
using AuthService.MapperProfiles;
using AuthService.Model;
using AuthService.Repositories;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace AuthService
{
    public static class AuthExtensions
    {
        public static IServiceCollection AuthServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();


            services.AddDbContext<AuthDbContext>(cfg =>
            {
                cfg.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentityCore<ApplicationUser>(cfg =>
            {
                cfg.Password.RequiredLength = 8;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager();

            services.AddFluentEmail(
                configuration["FluentEmail:SenderEmail"],
                configuration["FluentEmail:Sender"])
                .AddSmtpSender(configuration["FluentEmail:Host"], configuration.GetValue<int>("FluentEmail:Port"));

            // DI Configurations
            services.AddScoped<IUserManagerQueryService, UserManagerQueryService>();
            services.AddScoped<IUserRoleQueryService, UserRoleQueryService>();
            services.AddScoped<IUserSignInManagerService, UserSignInManagerService>();
            services.AddSwaggerGen();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).
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
                options.AddPolicy("AdminOnly", policy => policy.RequireRole(Roles.SuperAdmin, Roles.Admin));
            });


            services.AddAutoMapper(options =>
            {
                options.AddProfile<UserProfile>();
            });
            services.AddOpenApi();
            services.AddCarter();
            return services;
        }
    }
}

