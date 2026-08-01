using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Shared.Filters;

namespace Shared.Extension;

public static class ValidationFilterExtensions
{
    public static RouteHandlerBuilder Validate<T>(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<ValidationFilter<T>>();
    }
}