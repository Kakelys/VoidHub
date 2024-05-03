using ForumApi.Data.Models;
using ForumApi.DTO.Utils;
using ForumApi.Utils.Extensions;
using ForumApi.Controllers.Filters;
using ForumApi.Services.ForumS.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.Services.FileS.Interfaces;
using ForumApi.DTO.DPost;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/posts")]
    public class PostController(
        IPostService _postService,
        IRepositoryManager _rep,
        IFileService _fileService,
        ILikeService likeService
    ) : ControllerBase
    {
        [HttpGet("{id}/comments")]
        [Authorize]
        [AllowAnonymous]
        public async Task<IActionResult> GetPage(int id, [FromQuery] Offset offset, [FromQuery] Params prms)
        {
            var res = await _postService.GetPostComments(id, offset, new Params
            {
                BelowTime = prms.BelowTime
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

        [HttpGet("{id}/images")]
        [Authorize]
        public async Task<IActionResult> GetPostImages(int id)
        {
            return Ok(await _fileService.Get(id));
        }

        [HttpPost]
        [Authorize]
        [BanFilter]
        public async Task<IActionResult> Create([FromBody] PostEditDto postDto)
        {
            await _rep.BeginTransaction();
            try
            {
                var post = await _postService.Create(User.GetId(), postDto);
                // update files post ids
                if(postDto.FileIds.Count > 0)
                    await _fileService.Update([..postDto.FileIds], post.Id);

                await _rep.Save();
                await _rep.Commit();

                return Ok(post);
            }
            catch
            {
                await _rep.Rollback();
                throw;
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        [PermissionActionFilter<Post>]
        [BanFilter]
        public async Task<IActionResult> Update(int id, [FromBody] PostEditDto postDto)
        {
            var post = await _postService.Update(id, postDto);
            return Ok(post);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{Role.Admin},{Role.Moder}")]
        [BanFilter]
        public async Task<IActionResult> DeleteAsDmin(int id)
        {
            await _postService.Delete(id);
            return Ok();
        }

        [HttpPost("{id}/add-like")]
        [Authorize]
        [BanFilter]
        public async Task<IActionResult> AddLike(int id)
        {
            await likeService.Like(User.GetId(), id);
            return Ok();
        }

        [HttpDelete("{id}/rem-like")]
        [Authorize]
        [BanFilter]
        public async Task<IActionResult> RemoveLike(int id)
        {
            await likeService.UnLike(User.GetId(), id);
            return Ok();
        }
    }
}