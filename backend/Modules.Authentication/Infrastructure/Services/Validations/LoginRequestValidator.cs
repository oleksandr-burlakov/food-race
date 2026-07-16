using FluentValidation;
using Modules.Authentication.Infrastructure.Services.DTOs;

namespace Modules.Authentication.Infrastructure.Services.Validations;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Login).NotEmpty().WithMessage("Login is required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}