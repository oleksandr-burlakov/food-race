using Shared.Errors;

namespace Modules.Authentication.Domain.Errors;

public static class AuthErrors
{
    public static CustomError UserNotFoundOrRefreshTokenInvalid = CustomError.NotFound("Auth.RefreshToken",
        "User not found or Refresh Token is invalid.");

    public static CustomError InvalidAccessToken = CustomError.Validation("Auth.RefreshToken", "Invalid Access Token.");
}