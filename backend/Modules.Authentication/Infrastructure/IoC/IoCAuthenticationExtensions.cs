using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Modules.Authentication.Api;
using Modules.Authentication.Domain;
using Modules.Authentication.Infrastructure.DB;
using Modules.Authentication.Infrastructure.Services.Implementation;
using Shared.Extension;

namespace Modules.Authentication.Infrastructure.IoC;

public static class IoCAuthenticationExtensions
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var authAssembly = typeof(AuthModule).Assembly;
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtOptions = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>() ?? throw new InvalidOperationException("JwtOptions section is missing.");
        var key = Encoding.UTF8.GetBytes(jwtOptions.Secret);
        services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseNpgsql(connectionString, b => b.MigrationsAssembly(authAssembly))
                    .UseSnakeCaseNamingConvention())
            .AddValidatorsFromAssembly(authAssembly)
            .AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();
        services
            .AddScoped<IAuthService, AuthService>()
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Set to true in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.ValidIssuer,
                    ValidAudience = jwtOptions.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });
        services.AddAuthorization();

        services.AddCarterAssembly(authAssembly);
        return services;
    }
}