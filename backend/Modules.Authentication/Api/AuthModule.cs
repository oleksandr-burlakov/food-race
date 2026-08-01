using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Authentication.Infrastructure.Services.DTOs;
using Modules.Authentication.Infrastructure.Services.Implementation;
using Shared.Extension;

namespace Modules.Authentication.Api;

public class AuthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .RequireFeatureFlag("authentication");

        group.MapPost("/register",
                async (RegisterRequest request, IAuthService authService) =>
                    await authService.RegisterAsync(request))
            .Validate<RegisterRequest>();
        group.MapPost("/login", async (LoginRequest request,
                    IAuthService service) =>
                (await service.LoginAsync(request)).ToHttpResponse())
            .Validate<LoginRequest>();

        group.MapPost("/refresh", async (RefreshTokenRequest request, IAuthService service) =>
                (await service.RefreshTokenAsync(request)).ToHttpResponse())
            .Validate<RefreshTokenRequest>();

        // TODO: implement in next version
        group.MapPost("/logout", () => Results.Ok());
        group.MapPost("/forgot", () => Results.Ok());
        group.MapPost("/reset", () => Results.Ok());
        group.MapGet("/me", () => Results.Ok(new
            {
                User = "me"
            }))
            .RequireAuthorization();
    }
}