using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Authentication.Infrastructure;
using Modules.Authentication.Infrastructure.Services.DTOs;
using Modules.Authentication.Infrastructure.Services.Implementation;

namespace Modules.Authentication.Api;

public class AuthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register",
                async (RegisterRequest request, IAuthService authService) =>
                    await authService.RegisterAsync(request))
            .AddEndpointFilter<ValidationFilter<RegisterRequest>>();
        group.MapPost("/login", async (LoginRequest request,
                    IAuthService service) =>
                await service.LoginAsync(request))
            .AddEndpointFilter<ValidationFilter<LoginRequest>>();

        group.MapPost("/refresh", () => Results.Ok());
        group.MapPost("/logout", () => Results.Ok());
        group.MapPost("/forgot", () => Results.Ok());
        group.MapPost("/reset", () => Results.Ok());

        group.MapGet("/me", () => Results.Ok(new
        {
            User = "me"
        }));
    }
}