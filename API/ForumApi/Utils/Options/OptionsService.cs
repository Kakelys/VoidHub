using ForumApi.Utils.Options;

namespace ForumApi.Options;

public static class OptionsService
{
    public static IServiceCollection AddAppOptions(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<JwtOptions>()
            .Bind(config.GetSection(JwtOptions.Jwt))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<ImageOptions>()
            .Bind(config.GetSection(ImageOptions.Image))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<EmailOptions>()
            .Bind(config.GetSection(EmailOptions.Email))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<UtilsOptions>()
            .Bind(config.GetSection(UtilsOptions.Utils))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}