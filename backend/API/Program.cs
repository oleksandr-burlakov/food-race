using API.IoC;
using Carter;
using Modules.Authentication.Infrastructure.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddUnleashServices(builder.Configuration)
    .AddAuthenticationServices(builder.Configuration)
    .AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.MapCarter();
app.Run();