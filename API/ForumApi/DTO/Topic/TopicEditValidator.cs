using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;
using ForumApi.Utils.Extensions;

namespace ForumApi.DTO.DTopic
{
    public class TopicEditValidator : AbstractValidator<TopicEdit>
    {
        public TopicEditValidator(IJsonStringLocalizer locale)
        {
            RuleFor(x => x.Title)
                .TopicTitleRules(locale);
        }
    }
}