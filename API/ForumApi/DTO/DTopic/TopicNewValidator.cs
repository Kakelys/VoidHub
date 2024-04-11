using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;

namespace ForumApi.DTO.DTopic
{
    public class TopicNewValidator : AbstractValidator<TopicNew>
    {
        public TopicNewValidator(IJsonStringLocalizer locale)
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(locale["validators.title-required"])
                .Length(3, 255)
                .WithMessage(locale["validators.title-length"]);

            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage(locale["validators.content-required"])
                .Length(1, 24000)
                .WithMessage(locale["validators.content-length"]);
                
        }
    }
}