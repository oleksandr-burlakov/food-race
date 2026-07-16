namespace Modules.Authentication.Infrastructure.Services.DTOs;

public record LoginRequest(string Login, string Password);

public record RegisterRequest(string Email, string Password, string UserName);

public record RefreshTokenRequest(
    string AccessToken,
    string RefreshToken);

public record AuthResponse(string AccessToken, string RefreshToken, DateTime Expiration);