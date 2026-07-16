using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Modules.Authentication.Infrastructure;

// TODO: move to shared library
public class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argument = context.Arguments.FirstOrDefault(x => x is T);
        if (argument is not null)
        {
            var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
            if (validator is not null)
            {
                var validationResult = await validator.ValidateAsync((T)argument);
                if (!validationResult.IsValid)
                    return Results.ValidationProblem(validationResult.ToDictionary());
            }
        }

        return await next(context);
    }
}