using FluentValidation;

namespace Modules.Authentication.Infrastructure.Services.DTOs;

public record RefreshTokenRequest(
    string AccessToken,
    string RefreshToken);

public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(r => r.RefreshToken).NotNull().WithMessage("RefreshToken is required");
        RuleFor(r => r.AccessToken).NotEmpty().WithMessage("AccessToken is required");
    }
}