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

public class PermissionActionFilter<T> : ActionFilterAttribute where T : class
{
    private readonly string _columnName = "AccountId";

    public PermissionActionFilter(string columnName)
    {
        _columnName = columnName;
    }

    public PermissionActionFilter() { }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userId = context.HttpContext.User.GetId();

        var locale = context.HttpContext.RequestServices.GetService<IJsonStringLocalizer>()
            ?? throw new NoNullAllowedException("JsonStringLocalizer");

        if (!context.ActionArguments.TryGetValue("id", out object? value))
            throw new BadRequestException(locale["errors.id-not-provided"]);

        if (!int.TryParse(value?.ToString(), out int entityId))
            throw new BadRequestException(locale["errors.id-not-valid"]);

        var db = context.HttpContext.RequestServices.GetService<ForumDbContext>()
            ?? throw new ArgumentNullException(locale["errors.no-db-context"]);

        _ = await db.Set<T>()
            .EnableAsTracking(false)
            .Where($"Id == {entityId} and {_columnName} == {userId}")
            .FirstOrDefaultAsync() ?? throw new ForbiddenException(locale["errors.no-permission"]);

        await next();
    }
}