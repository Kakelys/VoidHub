using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;
using ForumApi.Options;

namespace ForumApi.Utils.Extensions
{
    public static class RuleExtensions
    {
        public static IRuleBuilderOptions<T, string?> PasswordRules<T>(this IRuleBuilder<T, string?> ruleBuilder, IJsonStringLocalizer locale)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage(locale["validators.password-required"])
                .Length(8,32)
                .WithMessage(locale["validators.password-length"])
                .Matches(@"^[a-zA-Z\d!@#$%^&*()\-_=+{}:,<.>]+$")
                .WithMessage(locale["validators.password-characters"]);
        }

        public static IRuleBuilderOptions<T, string?> EmailRules<T>(this IRuleBuilder<T, string?> ruleBuilder, IJsonStringLocalizer locale)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage(locale["validators.email-required"])
                .EmailAddress()
                .WithMessage(locale["validators.email-valid"]);
        }

        public static IRuleBuilderOptions<T, string?> UsernameRules<T>(this IRuleBuilder<T, string?> ruleBuilder, IJsonStringLocalizer locale)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage(locale["validators.username-required"])
                .Length(3, 32)
                .WithMessage(locale["validators.username-length"])
                .Matches(@"^[a-zA-Z\d!#$%^&*()\-_=+{}:,<.>]+$")
                .WithMessage(locale["validators.username-characters"]);
        }

        public readonly static string[] imageDefaultExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        public static IRuleBuilderOptions<T, IFormFile?> ImgRules<T>(this IRuleBuilder<T, IFormFile?> ruleBuilder, IJsonStringLocalizer locale, ImageOptions imgOptions, string[]? extensions = null)
        {
            extensions ??= imageDefaultExtensions;

            return ruleBuilder
                .Must(i => i != null)
                .WithMessage(locale["validators.image-required"])
                .Must(i => extensions.Contains(Path.GetExtension(i!.FileName)))
                .WithMessage($"{locale["validators.image-format"]}: {string.Join(", ", extensions)}")
                .Must(i => i?.Length < imgOptions.ImageMaxSize)
                .WithMessage($"{locale["validators.image-size"]} {imgOptions.ImageMaxSize / 1024} KB");
        }

        public static IRuleBuilderOptions<T, string?> TopicTitleRules<T>(this IRuleBuilder<T, string?> ruleBuilder, IJsonStringLocalizer locale)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage(locale["validators.title-required"])
                .Length(3, 255)
                .WithMessage(locale["validators.title-length"]);
        }
    }
}