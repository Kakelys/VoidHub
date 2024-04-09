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
using ForumApi.Data.Repository.Interfaces;
using ForumApi.Services.FileS.Interfaces;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/topics")]
    public class TopicController(
        ITopicService topicService,
        IPostService postService,
        ISearchService searchService,
        IRepositoryManager rep,
        IFileService fileService,
        ILikeService likeService
    ) : ControllerBase
    {
        [Authorize]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTopics([FromQuery] Offset offset, [FromQuery] DateTime time)
        {
            var res = await topicService.GetTopics(offset, new Params
            {
                BelowTime = time
            });

            if(User.Identity?.IsAuthenticated == true)
            {
                var userId = User.GetId();
                foreach(var post in res)
                {
                    await likeService.UpdateLikeStatus(userId, post.Post);
                }
            }

            return Ok(res);
        }

        [Authorize]
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTopic(int id, [FromQuery] Offset offset)
        {
            var allowDeleted = false;
            var isAuthed = User.Identity?.IsAuthenticated == true;
            if(isAuthed)
            {
                if(User.IsInRole(Role.Admin) || User.IsInRole(Role.Moder))
                    allowDeleted = true;
            }

            var topic = await topicService.GetTopic(id, allowDeleted);
            if(topic == null)
                return NotFound();

            topic.Posts = await postService.GetPostComments(topic.Post.Id, offset, new Params{IncludeDeleted = allowDeleted});
            if(isAuthed)
            {
                var userId = User.GetId();
                await likeService.UpdateLikeStatus(userId, topic.Post);

                foreach(var post in topic.Posts)
                {
                    await likeService.UpdateLikeStatus(userId, post.Post);
                }
            }

            return Ok(topic);
        }

        [Authorize]
        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] SearchDto search, [FromQuery] SearchParams searchParams, [FromQuery] Page page)
        {
            var isAuthed = User.Identity?.IsAuthenticated == true;
            var prms = new SearchParams
            {
                Sort = searchParams.Sort,
                WithPostContent = searchParams.WithPostContent
            };

            if(isAuthed && (User.IsInRole(Role.Admin) || User.IsInRole(Role.Moder)))
            {
                prms.OnlyDeleted = searchParams.OnlyDeleted;
            }

            var res = await searchService.SearchTopics(search.Query, prms, page);
            if(isAuthed)
            {
                var userId = User.GetId();
                foreach(var post in res.Data)
                {
                    await likeService.UpdateLikeStatus(userId, post.Post);
                }
            }

            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [BanFilter]
        public async Task<IActionResult> Create(TopicNew topicDto)
        {
            await rep.BeginTransaction();
            try
            {
                var res = await topicService.Create(User.GetId(), topicDto);
                // update files post ids
                if(topicDto.FileIds.Count > 0)
                    await fileService.Update([..topicDto.FileIds], res.Post.Id);

                await rep.Save();
                await rep.Commit();

                return Ok(res.Topic);
            }
            catch
            {
                await rep.Rollback();
                throw;
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{Role.Admin},{Role.Moder}")]
        [BanFilter]
        public async Task<IActionResult> Update(int id, TopicEdit topicDto)
        {
            var topic = await topicService.Update(id, topicDto);
            return Ok(topic);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{Role.Admin},{Role.Moder}")]
        [BanFilter]
        public async Task<IActionResult> Delete(int id)
        {
            await topicService.Delete(id);
            return Ok();
        }

        [HttpPatch("{id}/recover")]
        [Authorize(Roles = $"{Role.Admin},{Role.Moder}")]
        [BanFilter]
        public async Task<IActionResult> Recover(int id)
        {
            var dto = await topicService.Recover(id);
            return Ok(dto);
        }
    }
}