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
using ForumApi.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using ForumApi.Utils.Exceptions;
using AspNetCore.Localizer.Json.Localizer;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/accounts")]
    public class AccountController(
        IAccountService accountService,
        IOptions<ImageOptions> options,
        IImageService imageService,
        IPostService postService,
        ITopicService topicService,
        IRepositoryManager rep,
        ILikeService likeService,
        IJsonStringLocalizer locale) : ControllerBase
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
            if(User.IsAdminOrModer())
            {
                includeDeleted = true;
            }

            var prms = new Params()
            {
                BelowTime = belowTime,
                IncludeDeleted = includeDeleted,
                ByAccountId = id,
                OrderBy = "CreatedAt desc"
            };

            var res = await postService.GetPosts(offset, prms);
            if(User.IsAuthed())
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
        [HttpGet("{id}/topics")]
        public async Task<IActionResult> GetAccountTopics(int id, [FromQuery] Offset offset, [FromQuery] DateTime belowTime)
        {
            var includeDeleted = false;
            if(User.IsAdminOrModer())
            {
                includeDeleted = true;
            }

            var prms = new Params()
            {
                BelowTime = belowTime,
                IncludeDeleted = includeDeleted,
                ByAccountId = id,
                OrderBy = "CreatedAt desc"
        };

            var res = await topicService.GetTopics(offset, prms);
            if(User.IsAuthed())
            {
                var userId = User.GetId();
                foreach(var data in res)
                {
                    await likeService.UpdateLikeStatus(userId, data.Post);
                }
            }

            return Ok(res);
        }

        [HttpPatch]
        [Authorize]
        [BanFilter]
        public async Task<IActionResult> UpdateSelf([FromBody] AccountDto accountDto)
        {
            var validator = new AccountDtoValidator(locale);
            await validator.ValidateAndThrowAsync(accountDto);

            var senderId = User.GetId();
            return Ok(await accountService.Update(senderId, senderId, accountDto));
        }

        [HttpPatch("avatar")]
        [Authorize]
        [BanFilter]
        public async Task<IActionResult> UpdateImageSelf([FromBody] AccountDto accountDto)
        {
            var validator = new AccountDtoImageValidator(options, locale);
            await validator.ValidateAndThrowAsync(accountDto);

            var avatarPath = $"{_imageOptions.AvatarFolder}/{User.GetId()}{_imageOptions.FileType}";
            var fullPath = Path.Combine(_imageOptions.Folder, avatarPath);

            var image = imageService.Load(accountDto.Img);
            imageService.ResizeWithAspect(image, _imageOptions.ResizeWidth, _imageOptions.ResizeHeight);
            imageService.Crop(image);
            await imageService.SaveImage(image, fullPath);

            return Ok(await accountService.UpdateImg(User.GetId(), avatarPath));
        }

        [HttpPatch("{username}/role")]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> ChangeRole(string username, [FromBody] AccountDto accountDto)
        {
            var validator = new AccountDtoAdminValidator(locale);
            await validator.ValidateAndThrowAsync(accountDto);

            var user = await rep.Account.Value
                .FindByUsername(username)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("User not found");

            await accountService.Update(user.Id, User.GetId(), accountDto);
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

        [HttpPatch("{username}/rename")]
        [Authorize(Roles = $"{Role.Admin},{Role.Moder}")]
        [BanFilter]
        public async Task<IActionResult> RenameAccount(string username, [FromBody] AccountDto accountDto)
        {
            var validator = new AccountDtoAdminUsernameValidator(locale);
            await validator.ValidateAndThrowAsync(accountDto);

            var user = await rep.Account.Value
                .FindByUsername(username)
                .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-user"]);

            await accountService.Update(user.Id, User.GetId(), accountDto);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            await accountService.Delete(id);
            return Ok();
        }

        [HttpDelete]
        [BanFilter]
        [Authorize]
        public async Task<IActionResult> DeleteSelf()
        {
            await accountService.Delete(User.GetId());
            return Ok();
        }
    }
}