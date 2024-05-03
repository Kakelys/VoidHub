using ForumApi.Data.Models;
using ForumApi.DTO.DForum;
using ForumApi.DTO.Utils;
using ForumApi.Controllers.Filters;
using ForumApi.Services.ForumS.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ForumApi.Utils.Extensions;
using Microsoft.Extensions.Options;
using ForumApi.Options;
using ForumApi.Services.Utils.Interfaces;
using ForumApi.Data.Repository.Interfaces;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/forums")]
    public class ForumController(
        IForumService forumService,
        ITopicService topicService,
        IOptions<ImageOptions> imgOptions,
        IImageService imageService,
        IRepositoryManager rep) : ControllerBase
    {
        private readonly ImageOptions _imgOptions = imgOptions.Value;


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

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> Create([FromBody] ForumEdit forumDto)
        {
            var forum = await forumService.Create(forumDto);
            return Ok(forum);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> Update(int id, [FromForm] ForumEdit forumDto)
        {
            var imagePath = "";
            if(forumDto.Img != null)
            {
                imagePath = $"{_imgOptions.ForumFolder}/{Guid.NewGuid()}{_imgOptions.FileType}";
                var fullPath = Path.Combine(_imgOptions.Folder, imagePath);

                var image = imageService.Load(forumDto.Img);
                imageService.ResizeWithAspect(image, _imgOptions.ResizeWidth, _imgOptions.ResizePostHeight);
                imageService.Crop(image);
                await imageService.SaveImage(image, fullPath);
            }

            var oldForum = rep.Forum.Value.FindByCondition(f => f.Id == id).FirstOrDefault();
            var forum = await forumService.Update(id, forumDto, imagePath);

            // delete img 
            if(!string.IsNullOrEmpty(imagePath)
            && oldForum!.ImagePath != imagePath
            && oldForum!.ImagePath != _imgOptions.ForumDefault
            && System.IO.File.Exists($"{_imgOptions.Folder}/{oldForum!.ImagePath}"))
            {
                System.IO.File.Delete($"{_imgOptions.Folder}/{oldForum!.ImagePath}");
            }

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