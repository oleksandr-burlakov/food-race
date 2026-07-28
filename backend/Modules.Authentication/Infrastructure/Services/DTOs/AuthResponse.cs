namespace Modules.Authentication.Infrastructure.Services.DTOs;

public record AuthResponse(string AccessToken, string RefreshToken, DateTime Expiration);