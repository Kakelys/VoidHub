using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;
using ForumApi.Options;
using ForumApi.Utils.Extensions;
using Microsoft.Extensions.Options;

namespace ForumApi.DTO.DForum
{
    public class ForumEditValidator : AbstractValidator<ForumEdit>
    {
        public ForumEditValidator(IJsonStringLocalizer locale, IOptions<ImageOptions> imgOptions)
        {
            RuleFor(f => f.Title)
                .Length(3, 255)
                .WithName(locale["validators.title-length"]);

            RuleFor(f => f.SectionId)
                .GreaterThan(0)
                .WithMessage(locale["validators.sectionid-invalid"]);

            When(f => f.Img != null, () => {
                RuleFor(f => f.Img).ImgRules(locale, imgOptions.Value);
            });
        }
    }
}