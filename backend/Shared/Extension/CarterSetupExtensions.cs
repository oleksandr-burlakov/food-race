using System.Reflection;
using Carter;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Extension;

public static class CarterSetupExtensions
{
    public static IServiceCollection AddCarterAssembly(this IServiceCollection services, Assembly assembly)
    {
        return services.AddCarter(configurator: config =>
        {
            var moduleTypes = assembly.GetTypes()
                .Where(t => typeof(ICarterModule).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
                .ToArray();
            config.WithModules(moduleTypes);
        });
    }
}