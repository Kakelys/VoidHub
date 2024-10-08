using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;

namespace ForumApi.DTO.DBan;

public class BanEditValidator : AbstractValidator<BanEdit>
{
    public BanEditValidator(IJsonStringLocalizer locale)
    {
        RuleFor(b => b.Username)
            .NotEmpty()
            .WithMessage(locale["validators.username-required"]);

        RuleFor(b => b.Reason)
            .NotEmpty()
            .WithMessage(locale["validators.reason-required"]);

        RuleFor(b => b.ExpiresAt)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage(locale["validators.date-invalid"]);
    }
}