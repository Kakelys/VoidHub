using ForumApi.Data.Models;
using ForumApi.DTO.DSearch;
using ForumApi.DTO.DTopic;
using ForumApi.DTO.Utils;
using ForumApi.Utils.Extensions;
using ForumApi.Controllers.Filters;
using ForumApi.Services.ForumS.Interfaces;
using ForumApi.Services.Utils.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/topics")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;
        private readonly IPostService _postService;
        private readonly ISearchService _searchService;

        public TopicController(
            ITopicService topicService,
            IPostService postService,
            ISearchService searchService)
        {
            _topicService = topicService;
            _postService = postService;
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTopics([FromQuery] Offset offset, [FromQuery] DateTime time)
        {
            return Ok(await _topicService.GetTopics(offset, new Params 
            {
                BelowTime = time
            }));
        }

        [Authorize]
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTopic(int id, [FromQuery] Offset offset)
        {
            var allowDeleted = false;
            if(User.Identity != null && User.Identity.IsAuthenticated)
            {
                if(User.IsInRole(Role.Admin) || User.IsInRole(Role.Moder))
                    allowDeleted = true;
            }

            var topic = await _topicService.GetTopic(id, allowDeleted);
            if(topic == null)
                return NotFound();

            topic.Posts = await _postService.GetPostComments(topic.Post.Id, offset, allowDeleted);
            return Ok(topic);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] SearchDto search, [FromQuery] SearchParams searchParams, [FromQuery] Page page)
        {
            return Ok(await _searchService.SearchTopics(search.Query, searchParams, page));
        }

        [HttpPost]
        [Authorize]
        [BanFilter]
        public async Task<IActionResult> Create(TopicNew topicDto)
        {
            var topic = await _topicService.Create(User.GetId(), topicDto);
            return Ok(topic);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{Role.Admin},{Role.Moder}")]
        [BanFilter]
        public async Task<IActionResult> Update(int id, TopicDto topicDto)
        {
            var topic = await _topicService.Update(id, topicDto);
            return Ok(topic);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{Role.Admin},{Role.Moder}")]
        [BanFilter]
        public async Task<IActionResult> Delete(int id)
        {
            await _topicService.Delete(id);
            return Ok();
        }
    }
}