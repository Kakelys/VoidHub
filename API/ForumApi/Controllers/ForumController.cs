using ForumApi.Data.Models;
using ForumApi.DTO.DForum;
using ForumApi.DTO.Utils;
using ForumApi.Controllers.Filters;
using ForumApi.Services.ForumS.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ForumApi.DTO.DSearch;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/forums")]
    public class ForumController : ControllerBase
    {
        private readonly IForumService _forumService;
        private readonly ITopicService _topicService;

        public ForumController(
            IForumService forumService,
            ITopicService topicService)
        {
            _forumService = forumService;
            _topicService = topicService;
        }

        [Authorize]
        [AllowAnonymous]
        [HttpGet("{forumId}")]
        public async Task<IActionResult> GetForum(int forumId, [FromQuery] Params prms)
        {
            var isAuthed = User.Identity != null && User.Identity.IsAuthenticated;
            if(!isAuthed || !(User.IsInRole(Role.Admin) || User.IsInRole(Role.Moder)))
                prms = Params.FromUser(prms);
            
            return Ok(await _forumService.Get(forumId, prms));
        }

        [Authorize]
        [AllowAnonymous]
        [HttpGet("{forumId}/topics")]
        public async Task<IActionResult> GetTopicsPage(int forumId, [FromQuery] Page page, [FromQuery] Params prms)
        {
            var isAuthed = User.Identity != null && User.Identity.IsAuthenticated;
            if(!isAuthed || !(User.IsInRole(Role.Admin) || User.IsInRole(Role.Moder)))
                prms = Params.FromUser(prms);

            return Ok(await _topicService.GetTopics(forumId, page, prms));
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> Create(ForumEdit forumDto)
        {
            var forum = await _forumService.Create(forumDto);
            return Ok(forum);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> Update(int id, ForumEdit forumDto)
        {
            var forum = await _forumService.Update(id, forumDto);
            return Ok(forum);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> Delete(int id)
        {
            await _forumService.Delete(id);
            return Ok();
        }
    }
}