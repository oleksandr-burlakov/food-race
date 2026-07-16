using Unleash;

namespace API.IoC;

public static class IoCApiExtensions
{
    public static IServiceCollection AddUnleashServices(this IServiceCollection services, IConfiguration configuration)
    {
        var customSettings = new UnleashSettings();

        configuration.Bind(customSettings);
        configuration.GetSection(UnleashSettings.SectionName).Bind(customSettings);

        if (customSettings is { ApiUrl: not null, AuthorizationToken: not null })
        {
            var settings = new Unleash.UnleashSettings
            {
                AppName = customSettings.AppName,
                UnleashApi = new Uri(customSettings.ApiUrl),
                CustomHttpHeaders = new Dictionary<string, string>
                {
                    { "Authorization", customSettings.AuthorizationToken }
                }
            };

            var unleash = new DefaultUnleash(settings);
            services.AddSingleton<IUnleash>(unleash);
        }
        else
        {
            throw new MissingFieldException(
                $"Missing {nameof(customSettings.ApiUrl)} or {nameof(customSettings.AuthorizationToken)} in  {nameof(UnleashSettings)}.");
        }

        return services;
    }
}