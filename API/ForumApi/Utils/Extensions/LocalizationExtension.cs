using System.Globalization;
using System.Text;
using AspNetCore.Localizer.Json.Extensions;

namespace ForumApi.Utils.Extensions;

public static class LocalizationExtension
{
    public static IServiceCollection ConfigureLocalization(this IServiceCollection services)
    {
        var supportedCultures = new List<CultureInfo>
        {
            new ("en"),
            new ("ru")
        };

        services.AddJsonLocalization(options =>
        {
            options.CacheDuration = TimeSpan.FromMinutes(15);
            options.ResourcesPath = "Locales/i18n";
            options.LocalizationMode = AspNetCore.Localizer.Json.JsonOptions.LocalizationMode.I18n;
            options.FileEncoding = Encoding.GetEncoding("utf-8");
            options.UseBaseName = false;
            options.SupportedCultureInfos = [.. supportedCultures];
            options.DefaultCulture = new("en");
            options.DefaultUICulture = new("en");
        });

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture =
                new Microsoft.AspNetCore.Localization.RequestCulture("en");
            options.SupportedUICultures = supportedCultures;
        });

        return services;
    }
}