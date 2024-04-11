using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;

namespace ForumApi.DTO.Auth
{
    public class RegisterValidator : AbstractValidator<Register>
    {
        public RegisterValidator(IJsonStringLocalizer locale)
        {
            RuleFor(r => r.Email)
                .NotEmpty()
                .WithMessage(locale["validators.email-required"])
                .EmailAddress()
                .WithMessage(locale["validators.email-valid"]);

            RuleFor(r => r.Username)
                .NotEmpty()
                .WithMessage(locale["validators.username-required"])
                .Length(3, 32)
                .WithMessage(locale["validators.username-length"])
                .Matches(@"^[a-zA-Z\d!@#$%^&*()\-_=+{}:,<.>]+$")
                .WithMessage(locale["validators.username-characters"]);

            RuleFor(r => r.LoginName)
                .NotEmpty()
                .WithMessage(locale["validators.login-required"])
                .Matches(@"^[a-zA-Z]+$")
                .WithMessage(locale["validators.login-characters"])
                .Length(3, 32)
                .WithMessage(locale["validators.login-length"]);

            RuleFor(r => r.Password)
                .NotEmpty()
                .WithMessage(locale["validators.password-required"])
                .Length(8,32)
                .WithMessage(locale["validators.password-length"])
                .Matches(@"^[a-zA-Z\d!@#$%^&*()\-_=+{}:,<.>]+$")
                .WithMessage(locale["validators.password-characters"]);
        }
    }
}