using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;

namespace ForumApi.DTO.Utils
{
    public class OffsetValidator : AbstractValidator<Offset>
    {
        public OffsetValidator(IJsonStringLocalizer locale)
        {
            RuleFor(p => p.OffsetNumber)
                .GreaterThanOrEqualTo(0)
                .WithMessage(locale["validators.offsetnumber-invalid"]);

            RuleFor(p => p.Limit)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage(locale["validators.offsetlimit-size"]);
        }
    }
}