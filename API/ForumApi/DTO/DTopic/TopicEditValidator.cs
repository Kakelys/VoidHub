using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;

namespace ForumApi.DTO.DTopic
{
    public class TopicEditValidator : AbstractValidator<TopicEdit>
    {
        public TopicEditValidator(IJsonStringLocalizer locale)
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(locale["validators.title-required"])
                .Length(3, 255)
                .WithMessage(locale["validators.title-length"]);
        }        
    }
}