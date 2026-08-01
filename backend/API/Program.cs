using API.IoC;
using Carter;
using Modules.Authentication.Infrastructure.IoC;
using Serilog;
using Shared.ExceptionHandlers;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    await builder.Services.AddUnleashServices(builder.Configuration);
    builder.Services
        .AddAuthenticationServices(builder.Configuration)
        .AddOpenApi();

    var app = builder.Build();

    if (app.Environment.IsDevelopment()) app.MapOpenApi();

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapCarter();

    app.UseExceptionHandler();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}