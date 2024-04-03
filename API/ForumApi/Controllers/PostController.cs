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
        IFileService _fileService
    ) : ControllerBase
    {
        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetPage(int id, [FromQuery] Offset page)
        {
            var posts = await _postService.GetPostComments(id, page);
            return Ok(posts);
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
        public async Task<IActionResult> Create(PostEditDto postDto)
        {
            await _rep.BeginTransaction();
            try
            {
                var post = await _postService.Create(User.GetId(), postDto);
                // update files post ids
                if(postDto.FileIds.Count > 0)
                    await _fileService.Update(postDto.FileIds.ToArray(), post.Id);
                
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
        [BanFilter]
        [PermissionActionFilter<Post>]
        public async Task<IActionResult> Update(int id, PostEditDto postDto)
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
    }
}