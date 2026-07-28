using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Shared.Extension;

public static class UnleashFilterExtensions
{
    public static RouteHandlerBuilder RequireFeatureFlag(this RouteHandlerBuilder builder, string flagName)
    {
        return builder.AddEndpointFilter(new UnleashFeatureFilter(flagName));
    }

    public static RouteGroupBuilder RequireFeatureFlag(this RouteGroupBuilder builder, string flagName)
    {
        return builder.AddEndpointFilter(new UnleashFeatureFilter(flagName));
    }
}