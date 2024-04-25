using ForumApi.Data.Models;
using ForumApi.DTO.DForum;
using ForumApi.DTO.Utils;
using ForumApi.Controllers.Filters;
using ForumApi.Services.ForumS.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ForumApi.Utils.Extensions;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/forums")]
    public class ForumController(
        IForumService forumService,
        ITopicService topicService) : ControllerBase
    {
        [HttpGet("{forumId}")]
        [Authorize]
        [AllowAnonymous]
        public async Task<IActionResult> GetForum(int forumId, [FromQuery] Params prms)
        {
            if(!User.IsAdminOrModer())
            {
                prms = Params.FromUser(prms);
            }
            else
            {
                // TODO: better do on front
                prms.IncludeDeleted = true;
            }

            return Ok(await forumService.Get(forumId, prms));
        }

        [HttpGet("{forumId}/topics")]
        [Authorize]
        [AllowAnonymous]
        public async Task<IActionResult> GetTopicsPage(int forumId, [FromQuery] Page page, [FromQuery] Params prms)
        {
            var isAuthed = User.Identity?.IsAuthenticated == true;
            if(!isAuthed || !(User.IsInRole(Role.Admin) || User.IsInRole(Role.Moder)))
                prms = Params.FromUser(prms);

            return Ok(await topicService.GetTopics(forumId, page, prms));
        }

        [BanFilter]
        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> Create(ForumEdit forumDto)
        {
            var forum = await forumService.Create(forumDto);
            return Ok(forum);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> Update(int id, ForumEdit forumDto)
        {
            var forum = await forumService.Update(id, forumDto);
            return Ok(forum);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> Delete(int id)
        {
            await forumService.Delete(id);
            return Ok();
        }
    }
}