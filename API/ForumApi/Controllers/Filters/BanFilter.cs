using AutoMapper;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.DBan;
using ForumApi.Utils.Exceptions;
using ForumApi.Utils.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Controllers.Filters
{
    public class BanFilter : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userId = context.HttpContext.User.GetId();

            var banRepo = context.HttpContext.RequestServices.GetService<Lazy<IBanRepository>>()
                ?? throw new ArgumentNullException("Can't get IBanRepository");

            var userBan = await banRepo.Value
                .FindByCondition(b => b.AccountId == userId && b.IsActive && b.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(b => b.IsActive)
                .ThenByDescending(b => b.ExpiresAt)
                .Select(b => new BanResponse
                {
                    Id = b.Id,
                    Reason = b.Reason,
                    ExpiresAt = b.ExpiresAt
                })
                .FirstOrDefaultAsync();

            if(userBan != null)
            {
                throw new ForbiddenException(userBan);
            }

            await next();
        }
    }
}