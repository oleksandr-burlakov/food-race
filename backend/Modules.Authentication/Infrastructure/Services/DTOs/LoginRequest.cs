using FluentValidation;

namespace Modules.Authentication.Infrastructure.Services.DTOs;

public record LoginRequest(string Login, string Password);

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Login).NotEmpty().WithMessage("Login is required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}