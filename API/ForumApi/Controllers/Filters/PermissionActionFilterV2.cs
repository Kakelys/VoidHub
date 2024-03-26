using System.Linq.Dynamic.Core;
using ForumApi.Data;
using ForumApi.Data.Repository.Extensions;
using ForumApi.Utils.Exceptions;
using ForumApi.Utils.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Controllers.Filters
{
    /// <summary>
    /// Same as previous one, but allow to change primary column
    /// </summary>
    public class PermissionActionFilterV2<T>(string fieldName, string queryParamName, string userField = "AccountId") : ActionFilterAttribute where T : class
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userId = context.HttpContext.User.GetId();

            if (!context.ActionArguments.ContainsKey(queryParamName))
                throw new ArgumentNullException("Id is not provided");
            
            if(!int.TryParse(context.ActionArguments[queryParamName]?.ToString(), out int entityId))
                throw new BadRequestException("Id is not valid");

            var db = context.HttpContext.RequestServices.GetService<ForumDbContext>()
                ?? throw new Exception("Can't get db context");

            _ = await db.Set<T>()
                .EnableAsTracking(false)
                .Where($"{fieldName} == {entityId} and {userField} == {userId}")
                .FirstOrDefaultAsync() ?? throw new ForbiddenException("You don't have permission to do this action");

            await next();
        }
    }
}