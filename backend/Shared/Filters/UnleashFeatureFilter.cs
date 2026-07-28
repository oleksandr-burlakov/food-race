using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Unleash;

public class UnleashFeatureFilter : IEndpointFilter
{
    private readonly string _flagName;

    public UnleashFeatureFilter(string flagName)
    {
        _flagName = flagName;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var unleash = context.HttpContext.RequestServices.GetRequiredService<IUnleash>();

        if (!unleash.IsEnabled(_flagName))
            return Results.NotFound();

        return await next(context);
    }
}