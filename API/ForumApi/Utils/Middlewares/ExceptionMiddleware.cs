using System.Text.Json;
using AspNetCore.Localizer.Json.Localizer;
using ForumApi.Locales;
using ForumApi.Utils.Exceptions;

namespace ForumApi.Utils.Middlewares
{
    public class ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context, IJsonStringLocalizer locale)
        {
            try
            {
                await next(context);
            }
            catch(ArgumentNullException ex)
            {
                await HandleError(context, 400, $"{ex.Message} {locale["errors.is-empty"]}");
            }
            catch(BadRequestException ex)
            {
                await HandleError(context, 400, ex.Message);
            }
            catch(NotFoundException ex)
            {
                await HandleError(context, 404, ex.Message);
            }
            catch(ForbiddenException ex)
            {
                await HandleError(context, 403, ex.Message);
            }
            catch(FluentValidation.ValidationException ex)
            {
                var errorsObj = new { errors = ex.Errors.Select(e => new {e.PropertyName, e.ErrorMessage}) };

                await HandleError(context, 400, JsonSerializer.Serialize(errorsObj));
            }
            catch(Exception ex)
            {
                logger.LogError(ex, ex.Message);

                await HandleError(context, 500, locale["errors.internal-server-error"]);
            }
        }

        private static async Task HandleError(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(message);
        }
    }
}