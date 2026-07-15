using Unleash;
using Unleash.ClientFactory;

var builder = WebApplication.CreateBuilder(args);

var settings = new UnleashSettings()
{
    AppName = "food-race",
    UnleashApi = new Uri("http://127.0.0.1:4242/api/"),
    CustomHttpHeaders = new Dictionary<string, string>()
    {
        {"Authorization","default:development.unleash-insecure-api-token"}
    }
};

var unleash =  new DefaultUnleash(settings);
builder.Services.AddSingleton<IUnleash>(unleash);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/check-auth-flag", (IUnleash unleashService) =>
{
    if (unleashService.IsEnabled("authentication"))
    {
        return Results.Ok(new { status = "authentication is enabled" });
    }
    else
    {
        return Results.Ok(new { status = "authentication is disabled (or SDK is still fetching...)" });
    }
});

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}