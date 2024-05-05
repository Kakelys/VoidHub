using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;
using ForumApi.Utils.Extensions;

namespace ForumApi.DTO.DTopic
{
    public class TopicNewValidator : AbstractValidator<TopicNew>
    {
        public TopicNewValidator(IJsonStringLocalizer locale)
        {
            RuleFor(x => x.Title)
                .TopicTitleRules(locale);

            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage(locale["validators.content-required"])
                .Length(1, 24000)
                .WithMessage(locale["validators.content-length"]);
        }
    }
}