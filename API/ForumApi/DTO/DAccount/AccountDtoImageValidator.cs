using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;
using ForumApi.Options;
using Microsoft.Extensions.Options;

namespace ForumApi.DTO.DAccount
{
    /// <summary>
    /// Validate only image
    /// </summary>
    public class AccountDtoImageValidator : AbstractValidator<AccountDto>
    {
        private readonly string[] _extensions = {".jpg", ".jpeg", ".png", ".gif", ".bmp"};

        public AccountDtoImageValidator(IOptions<ImageOptions> imageOptions, IJsonStringLocalizer locale)
        {
            var imgOptions = imageOptions.Value;

            RuleFor(r => r.Img)
                .Configure(c => c.CascadeMode = CascadeMode.Stop)
                .Must(i => i != null)
                .WithMessage(locale["validators.image-required"])
                .Must(i => _extensions.Contains(Path.GetExtension(i.FileName)))
                .WithMessage($"{locale["validators.image-format"]}: {string.Join(", ", _extensions)}")
                .Must(i => i.Length < imgOptions.ImageMaxSize)
                .WithMessage($"{locale["validators.image-size"]} {imgOptions.ImageMaxSize / 1024} KB");
        }        
    }
}