using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;
using ForumApi.Utils.Extensions;

namespace ForumApi.DTO.Auth;

public class PasswordRecoverValidator : AbstractValidator<PasswordRecover>
{
    public PasswordRecoverValidator(IJsonStringLocalizer locale)
    {
        RuleFor(p => p.Base64Token)
            .NotEmpty()
            .WithMessage(locale["errors.invalid-token"]);

        RuleFor(p => p.Password)
            .PasswordRules(locale);
    }
}