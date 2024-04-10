using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;
using ForumApi.Options;
using Microsoft.Extensions.Options;

namespace ForumApi.DTO.DFile
{
    public class NewImageValidator : AbstractValidator<NewFileDto>
    {
        private readonly string[] _extensions = {".jpg", ".jpeg", ".png", ".gif", ".bmp"};

        public NewImageValidator(IOptions<ImageOptions> imageOptions, IJsonStringLocalizer locale) 
        {
            var imgOptions = imageOptions.Value;

            RuleFor(r => r.PostId)
                .Must(id => id == null || id > 0)
                .WithMessage(locale["validators.postid-invalid"]);

            RuleFor(r => r.File)
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