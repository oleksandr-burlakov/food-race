using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Modules.Authentication.Domain;
using Modules.Authentication.Infrastructure.Services.DTOs;
using Unleash;

namespace Modules.Authentication.Infrastructure.Services.Implementation;

public interface IAuthService
{
    Task<IResult> LoginAsync(LoginRequest request);
    Task<IResult> RefreshTokenAsync(RefreshTokenRequest request);
    Task<IResult> RegisterAsync(RegisterRequest request);
}

public class AuthService(
    IConfiguration configuration,
    UserManager<User> userManager,
    IUnleash unleash) : IAuthService
{
    public async Task<IResult> RegisterAsync(RegisterRequest request)
    {
        if (!unleash.IsEnabled(configuration["Features:AuthModule"]))
            return Results.BadRequest(new
            {
                Message = "This feature is disabled."
            });

        var userExists = await userManager.FindByEmailAsync(request.Email);
        if (userExists != null) return Results.BadRequest(new { Error = "User with same Email already  exists." });

        var user = new User
        {
            Email = request.Email,
            UserName = request.UserName,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return Results.BadRequest(new { Errors = result.Errors.Select(e => e.Description) });

        return Results.Ok(new { Message = "User successfully registered" });
    }

    public async Task<IResult> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByNameAsync(request.Login) ??
                   await userManager.FindByEmailAsync(request.Login);

        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            return Results.Unauthorized();

        var authResponse = await GenerateAuthResponseAsync(user);
        return Results.Ok(authResponse);
    }

    public async Task<IResult> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var principal = GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null) return Results.BadRequest(new { Error = "Invalid Access Token." });

        var login = principal.FindFirstValue(ClaimTypes.Email) ?? principal.FindFirstValue(ClaimTypes.Name);
        var user = await userManager.FindByEmailAsync(login) ?? await userManager.FindByNameAsync(login);

        // TODO: add refresh token lifetime validation check
        if (user == null)
            return Results.BadRequest(new { Error = "User not found or Refresh Token is invalid." });

        var newAuthResponse = await GenerateAuthResponseAsync(user);
        return Results.Ok(newAuthResponse);
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(User user)
    {
        var userRoles = await userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var userRole in userRoles) authClaims.Add(new Claim(ClaimTypes.Role, userRole));

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!));
        var tokenValidityInMinutes = int.Parse(configuration["JWT:TokenValidityInMinutes"] ?? "60");

        var token = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(authClaims),
            Expires = DateTime.UtcNow.AddMinutes(tokenValidityInMinutes),
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256Signature),
            Issuer = configuration["JWT:ValidIssuer"],
            Audience = configuration["JWT:ValidAudience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(token);
        var accessToken = tokenHandler.WriteToken(securityToken);

        var refreshToken = GenerateRefreshToken();


        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await userManager.UpdateAsync(user);

        return new AuthResponse(accessToken, refreshToken, token.Expires.Value);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            return null;

        return principal;
    }
}