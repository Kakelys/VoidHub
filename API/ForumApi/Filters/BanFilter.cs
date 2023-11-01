using AutoMapper;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.DBan;
using ForumApi.Exceptions;
using ForumApi.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Filters
{
    public class BanFilter : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userId = context.HttpContext.User.GetId();

            var banRepo = context.HttpContext.RequestServices.GetService<Lazy<IBanRepository>>()
                ?? throw new Exception("Can't get IBanRepository");

            var mapper = context.HttpContext.RequestServices.GetService<IMapper>()
                ?? throw new Exception("Can't get IMapper");

            var userBan = await banRepo.Value
                .FindByCondition(b => b.AccountId == userId && b.IsActive && b.ExpiresAt > DateTime.UtcNow)
                .OrderBy(b => b.ExpiresAt)
                .FirstOrDefaultAsync();


            if(userBan != null)
            {
                throw new ForbiddenException(mapper.Map<BanResponse>(userBan));
            }

            await next();
        }

    }
}