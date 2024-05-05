using AutoMapper;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.Auth;
using ForumApi.DTO.Notification;
using ForumApi.DTO.Stats;
using ForumApi.Services.Utils.Interfaces;
using ForumApi.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/stats")]
    public class StatsController(
        ISessionStorage sessionStorage, 
        IRepositoryManager rep,
        IMapper mapper) : ControllerBase
    {
        [HttpGet("online")]
        public IActionResult GetOnlineUsers()
        {
            return Ok(sessionStorage.GetOnlineStats());
        }

        [HttpGet("general")]
        //[ResponseCache(Duration = 300)]
        public async Task<IActionResult> GetGeneral()
        {
            var data = new GeneralStats 
            {
                TopicCount = rep.Topic.Value.FindAll().Count(),
                PostCount = rep.Post.Value.FindAll().Count(),
                UserCount = rep.Account.Value.FindAll().Count(),
                LastUser = mapper.Map<User>(rep.Account.Value
                    .FindByCondition(a => a.DeletedAt == null)
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefault())
            };

            return Ok(data);
        }
    }
}