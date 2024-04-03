using FluentValidation;
using ForumApi.Data.Models;
using ForumApi.DTO.DAccount;
using ForumApi.Utils.Extensions;
using ForumApi.Controllers.Filters;
using ForumApi.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ForumApi.Services.ForumS.Interfaces;
using ForumApi.Services.Utils.Interfaces;
using ForumApi.DTO.Utils;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/accounts")]
    public class AccountController(
        IAccountService accountService, 
        IOptions<ImageOptions> options, 
        IImageService imageService,
        IPostService postService,
        ITopicService topicService) : ControllerBase
    {   
        private readonly ImageOptions _imageOptions = options.Value;

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(int id)
        {
            return Ok(await accountService.Get(id));
        }

        [Authorize]
        [AllowAnonymous]
        [HttpGet("{id}/posts")]
        public async Task<IActionResult> GetAccountPosts(int id, [FromQuery] Offset offset, [FromQuery] DateTime belowTime)
        {
            var includeDeleted = false;
            if(User.Identity != null && User.Identity.IsAuthenticated)
            {
                if(User.IsInRole(Role.Admin) || User.IsInRole(Role.Moder))
                    includeDeleted = true;
            }

            var prms = new Params() 
            {
                BelowTime = belowTime,
                IncludeDeleted = includeDeleted,
                ByAccountId = id,
                OrderBy = "CreatedAt desc"
            };

            return Ok(await postService.GetPosts(offset, prms));
        }

         [Authorize]
        [AllowAnonymous]
        [HttpGet("{id}/topics")]
        public async Task<IActionResult> GetAccountTopics(int id, [FromQuery] Offset offset, [FromQuery] DateTime belowTime)
        {
            var includeDeleted = false;
            if(User.Identity != null && User.Identity.IsAuthenticated)
            {
                if(User.IsInRole(Role.Admin) || User.IsInRole(Role.Moder))
                    includeDeleted = true;
            }

            var prms = new Params() 
            {
                BelowTime = belowTime,
                IncludeDeleted = includeDeleted,
                ByAccountId = id,
                OrderBy = "CreatedAt desc"
            };

            return Ok(await topicService.GetTopics(offset, prms));
        }

        [HttpPatch]
        [Authorize]
        [BanFilter]
        public async Task<IActionResult> UpdateSelf([FromBody] AccountDto accountDto)
        {
            var validator = new AccountDtoValidator();
            await validator.ValidateAndThrowAsync(accountDto);

            var senderId = User.GetId();
            return Ok(await accountService.Update(senderId, senderId, accountDto));
        }

        [HttpPatch("avatar")]
        [Authorize]
        [BanFilter]
        public async Task<IActionResult> UpdateImageSelf(AccountDto accountDto, [FromQuery] string currentPath)
        {
            var validator = new AccountDtoImageValidator(options);
            await validator.ValidateAndThrowAsync(accountDto);

            var avatarPath = $"{_imageOptions.AvatarFolder}/{User.GetId()}{_imageOptions.FileType}";
            var fullPath = Path.Combine(_imageOptions.Folder, avatarPath);

            var image = imageService.Load(accountDto.Img);
            imageService.ResizeWithAspect(image, _imageOptions.ResizeWidth, _imageOptions.ResizePostHeight);
            imageService.Crop(image);
            await imageService.SaveImage(image, fullPath);

            return Ok(await accountService.UpdateImg(User.GetId(), avatarPath));
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] AccountDto accountDto)
        {
            var validator = new AccountDtoAdminValidator();
            await validator.ValidateAndThrowAsync(accountDto);

            await accountService.Update(id, User.GetId(), accountDto);
            return Ok();
        }

        [HttpPatch("{id}/avatar-default")]
        [Authorize(Roles = $"{Role.Admin},{Role.Moder}")]
        [BanFilter]
        public async Task<IActionResult> AvatarToDefault(int id)
        {
            await accountService.UpdateImg(id, _imageOptions.AvatarDefault);
            return Ok();
        }

        [HttpPatch("{id}/rename")]
        [Authorize(Roles = $"{Role.Admin},{Role.Moder}")]
        [BanFilter]
        public async Task<IActionResult> RenameAccount(int id, [FromBody] AccountDto accountDto)
        {
            var validator = new AccountDtoAdminUsernameValidator();
            await validator.ValidateAndThrowAsync(accountDto);

            await accountService.Update(id, User.GetId(), accountDto);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> DeleteAccount(int id) 
        {
            await accountService.Delete(User.GetId());
            return Ok();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteSelf()
        {
            await accountService.Delete(User.GetId());
            return Ok();
        }
    }
}