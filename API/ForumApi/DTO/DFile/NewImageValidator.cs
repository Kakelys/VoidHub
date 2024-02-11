using FluentValidation;
using ForumApi.Options;
using Microsoft.Extensions.Options;

namespace ForumApi.DTO.DFile
{
    public class NewImageValidator : AbstractValidator<NewFileDto>
    {
        private readonly string[] _extensions = {".jpg", ".jpeg", ".png", ".gif", ".bmp"};

        public NewImageValidator(IOptions<ImageOptions> imageOptions) 
        {
            var imgOptions = imageOptions.Value;

            RuleFor(r => r.File)
                .Configure(c => c.CascadeMode = CascadeMode.Stop)
                .Must(i => i != null)
                .WithMessage("Image cannot be empty")
                .Must(i => _extensions.Contains(Path.GetExtension(i.FileName)))
                .WithMessage($"Image must be in one of the following formats: {string.Join(", ", _extensions)}")
                .Must(i => i.Length < imgOptions.ImageMaxSize)
                .WithMessage($"Image too heavy, must be less than {imgOptions.ImageMaxSize / 1024} KB");
        }
    }
}