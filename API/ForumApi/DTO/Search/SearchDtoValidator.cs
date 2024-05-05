using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;

namespace ForumApi.DTO.DSearch
{
    public class SearchDtoValidator : AbstractValidator<SearchDto>
    {
        public SearchDtoValidator(IJsonStringLocalizer locale)
        {
            RuleFor(x => x.Query)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(locale["validators.query-required"])
                .Length(3, 255)
                .WithMessage(locale["validators.query-length"]);
        }
    }
}