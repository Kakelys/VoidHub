using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;
using ForumApi.Options;
using ForumApi.Utils.Extensions;
using Microsoft.Extensions.Options;

namespace ForumApi.DTO.DAccount
{
    /// <summary>
    /// Validate only image
    /// </summary>
    public class AccountDtoImageValidator : AbstractValidator<AccountDto>
    {
        public AccountDtoImageValidator(IOptions<ImageOptions> imageOptions, IJsonStringLocalizer locale)
        {
            var imgOptions = imageOptions.Value;

            RuleFor(r => r.Img)
                .Configure(c => c.CascadeMode = CascadeMode.Stop)
                .ImgRules(locale, imgOptions);
        }
    }
}