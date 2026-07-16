using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Authentication.Api;
using Modules.Authentication.Infrastructure.DB;
using Modules.Authentication.Infrastructure.Services.Implementation;

namespace Modules.Authentication.Infrastructure.IoC;

public static class IoCAuthenticationExtensions
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var authAssembly = typeof(AuthModule).Assembly;
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        return services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseNpgsql(connectionString, b => b.MigrationsAssembly(authAssembly))
                    .UseSnakeCaseNamingConvention())
            .AddValidatorsFromAssembly(authAssembly)
            .AddScoped<IAuthService, AuthService>();
    }
}