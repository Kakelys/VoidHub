using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;

namespace ForumApi.DTO.DSection;

public class SectionEditValidator : AbstractValidator<SectionEdit>
{
    public SectionEditValidator(IJsonStringLocalizer locale)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(locale["validators.title-required"])
            .Length(3, 255)
            .WithMessage(locale["validators.title-length"]);

        RuleFor(s => s.OrderPosition)
            .GreaterThanOrEqualTo(0)
            .WithMessage(locale["validators.orderposition-invalid"]);
    }
}