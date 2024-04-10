using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;

namespace ForumApi.DTO.DPost
{
    public class PostValidator : AbstractValidator<PostEditDto>
    {
        public PostValidator(IJsonStringLocalizer locale)
        {
            RuleFor(p => p.Content)
            .NotEmpty()
                .WithMessage(locale["validators.content-required"])
            .Length(1, 24000)
                .WithMessage(locale["validators.content-length"]);

            RuleFor(p => p.AncestorId)
                .Must(id => id == null || id > 0)
                .WithMessage(locale["validators.ancestorid-invalid"]);
        }
    }
}