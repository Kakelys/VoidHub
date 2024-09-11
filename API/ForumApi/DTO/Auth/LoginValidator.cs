using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;

namespace ForumApi.DTO.Auth;

public class LoginValidator : AbstractValidator<Login>
{
    public LoginValidator(IJsonStringLocalizer locale)
    {
        RuleFor(l => l.LoginName)
            .NotEmpty()
            .WithMessage(locale["validators.login-required"]);

        RuleFor(l => l.Password)
            .NotEmpty()
            .WithMessage(locale["validators.password-required"]);
    }
}