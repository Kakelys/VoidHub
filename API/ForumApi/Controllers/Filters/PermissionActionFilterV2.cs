using System.Data;
using System.Linq.Dynamic.Core;
using AspNetCore.Localizer.Json.Localizer;
using ForumApi.Data;
using ForumApi.Data.Repository.Extensions;
using ForumApi.Utils.Exceptions;
using ForumApi.Utils.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Controllers.Filters;

/// <summary>
/// Same as previous one, but allow to change primary column
/// </summary>
public class PermissionActionFilterV2<T>(string fieldName, string queryParamName, string userField = "AccountId") : ActionFilterAttribute where T : class
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userId = context.HttpContext.User.GetId();

        var locale = context.HttpContext.RequestServices.GetService<IJsonStringLocalizer>()
            ?? throw new NoNullAllowedException("JsonStringLocalizer");

        if (!context.ActionArguments.TryGetValue(queryParamName, out object value))
            throw new ArgumentNullException(locale["errors.id-not-provided"]);

        if (!int.TryParse(value?.ToString(), out int entityId))
            throw new BadRequestException(locale["errors.id-not-valid"]);

        var db = context.HttpContext.RequestServices.GetService<ForumDbContext>()
            ?? throw new ArgumentNullException(locale["errors.no-db-context"]);

        _ = await db.Set<T>()
            .EnableAsTracking(false)
            .Where($"{fieldName} == {entityId} and {userField} == {userId}")
            .FirstOrDefaultAsync() ?? throw new ForbiddenException(locale["errors.no-permission"]);

        await next();
    }
}