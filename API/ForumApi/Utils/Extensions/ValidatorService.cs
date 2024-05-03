using FluentValidation;
using FluentValidation.AspNetCore;
using ForumApi.DTO.Auth;
using ForumApi.DTO.DBan;
using ForumApi.DTO.DChat;
using ForumApi.DTO.DFile;
using ForumApi.DTO.DForum;
using ForumApi.DTO.DPost;
using ForumApi.DTO.DSearch;
using ForumApi.DTO.DSection;
using ForumApi.DTO.DTopic;
using ForumApi.DTO.Utils;

namespace ForumApi.Utils.Extensions
{
    public static class ValidatorService
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation(fv => fv.DisableDataAnnotationsValidation = true);

            services.AddScoped<IValidator<Page>, PageValidator>();
            services.AddScoped<IValidator<Offset>, OffsetValidator>();

            services.AddScoped<IValidator<Register>, RegisterValidator>();
            services.AddScoped<IValidator<Login>, LoginValidator>();
            services.AddScoped<IValidator<PasswordRecover>, PasswordRecoverValidator>();

            services.AddScoped<IValidator<SearchDto>, SearchDtoValidator>();
            services.AddScoped<IValidator<SearchParams>, SearchParamsValidator>();

            services.AddScoped<IValidator<SectionEdit>, SectionEditValidator>();

            services.AddScoped<IValidator<ForumEdit>, ForumEditValidator>();

            services.AddScoped<IValidator<TopicEdit>, TopicEditValidator>();
            services.AddScoped<IValidator<TopicNew>, TopicNewValidator>();

            services.AddScoped<IValidator<PostEditDto>, PostEditValidator>();

            services.AddScoped<IValidator<NewFileDto>, NewImageValidator>();

            services.AddScoped<IValidator<BanEdit>, BanEditValidator>();

            services.AddScoped<IValidator<Message>, MessageValidator>();
            return services;
        }
    }
}