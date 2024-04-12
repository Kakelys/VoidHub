using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;

namespace ForumApi.DTO.DForum
{
    public class ForumDtoValidator : AbstractValidator<ForumEdit>
    {
        public ForumDtoValidator(IJsonStringLocalizer locale)
        {
            RuleFor(f => f.Title)
                .NotEmpty()
                .WithName(locale["validators.title-required"])
                .Length(3, 255)
                .WithName(locale["validators.title-length"]);

            RuleFor(f => f.SectionId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(locale["validators.sectionid-invalid"]);
        }
    }
}