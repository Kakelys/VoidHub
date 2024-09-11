using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;
using ForumApi.Options;
using ForumApi.Utils.Extensions;
using Microsoft.Extensions.Options;

namespace ForumApi.DTO.DFile;

public class NewImageValidator : AbstractValidator<NewFileDto>
{
    public static readonly string[] supportedVideoFormats = [".mp4"];

    public NewImageValidator(IOptions<ImageOptions> imageOptions, IJsonStringLocalizer locale)
    {
        var imgOptions = imageOptions.Value;

        RuleFor(r => r.PostId)
            .Must(id => id == null || id > 0)
            .WithMessage(locale["validators.postid-invalid"]);

        RuleFor(r => r.File)
            .Configure(c => c.CascadeMode = CascadeMode.Stop)
            .ImgRules(
                locale,
                imgOptions,
                [.. RuleExtensions.imageDefaultExtensions, .. supportedVideoFormats]
            );
    }
}
