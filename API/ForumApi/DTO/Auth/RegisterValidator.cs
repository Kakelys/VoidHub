using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;
using ForumApi.Utils.Extensions;

namespace ForumApi.DTO.Auth;

public class RegisterValidator : AbstractValidator<Register>
{
    public RegisterValidator(IJsonStringLocalizer locale)
    {
        RuleFor(r => r.Email)
            .EmailRules(locale);

        RuleFor(r => r.Username)
            .UsernameRules(locale);

        RuleFor(r => r.LoginName)
            .NotEmpty()
            .WithMessage(locale["validators.login-required"])
            .Matches("^[a-zA-Z]+$")
            .WithMessage(locale["validators.login-characters"])
            .Length(3, 32)
            .WithMessage(locale["validators.login-length"]);

        RuleFor(r => r.Password)
            .PasswordRules(locale);
    }
}