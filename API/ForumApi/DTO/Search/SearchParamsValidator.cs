using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;

namespace ForumApi.DTO.DSearch;

public class SearchParamsValidator : AbstractValidator<SearchParams>
{
    public SearchParamsValidator(IJsonStringLocalizer locale)
    {
        RuleFor(x => x.Sort)
            .Configure(c => c.CascadeMode = CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(locale["validators.sort-empty"])
            .IsInEnum()
            .WithMessage(locale["validators.wrong-sort-param"]);
    }
}