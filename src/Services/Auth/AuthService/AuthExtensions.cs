using AuthService.Data;
using AuthService.Interfaces;
using AuthService.MapperProfiles;
using AuthService.Model;
using AuthService.OptionProperties;
using AuthService.Options;
using AuthService.Repositories;
using BuildingBlocks.Behaviors;
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
        public static IServiceCollection AuthServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddControllers();

            // IOption<T> configurations
            services.Configure<FluentEmailOptions>(configuration.GetSection("FluentEmail"));
            services.Configure<EmailConfirmationUrlOptions>(configuration.GetSection("EmailConfirmationUrl"));

            services.AddDbContext<AuthDbContext>(cfg =>
            {
                cfg.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentityCore<ApplicationUser>(cfg =>
            {
                cfg.Password.RequiredLength = 8;
                cfg.Password.RequireUppercase = true;
                cfg.Password.RequireLowercase = true;
                cfg.Password.RequireNonAlphanumeric = true;
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
            services.AddScoped<IUserManagerCommandService, UserManagerCommandService>();
            services.AddScoped<IUserRoleQueryService, UserRoleQueryService>();
            services.AddScoped<IUserSignInManagerService, UserSignInManagerService>();
            services.AddScoped<IJsonWebTokenService, JsonWebTokenService>();
            services.AddScoped<IFluentEmailSender, FluentEmailSender>();
            services.AddSwaggerGen();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
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

