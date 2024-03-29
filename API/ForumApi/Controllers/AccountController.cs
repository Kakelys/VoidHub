using FluentValidation;
using ForumApi.Data.Models;
using ForumApi.DTO.DAccount;
using ForumApi.Utils.Extensions;
using ForumApi.Controllers.Filters;
using ForumApi.Options;
using ForumApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/accounts")]
    public class AccountController(IAccountService _accountService, IOptions<ImageOptions> options, IImageService _imageService) : ControllerBase
    {   
        private readonly ImageOptions _imageOptions = options.Value;

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(int id)
        {
            return Ok(await _accountService.Get(id));
        }

        [HttpPatch]
        [Authorize]
        [BanFilter]
        public async Task<IActionResult> UpdateSelf([FromBody] AccountDto accountDto)
        {
            var validator = new AccountDtoValidator();
            await validator.ValidateAndThrowAsync(accountDto);

            var senderId = User.GetId();
            return Ok(await _accountService.Update(senderId, senderId, accountDto));
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

            var image = _imageService.Load(accountDto.Img);
            _imageService.Resize(image, _imageOptions.ResizeWidth, _imageOptions.ResizePostHeight);
            _imageService.Crop(image);
            await _imageService.SaveImage(image, fullPath);

            return Ok(await _accountService.UpdateImg(User.GetId(), avatarPath));
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] AccountDto accountDto)
        {
            var validator = new AccountDtoAdminValidator();
            await validator.ValidateAndThrowAsync(accountDto);

            await _accountService.Update(id, User.GetId(), accountDto);
            return Ok();
        }

        [HttpPatch("{id}/avatar-default")]
        [Authorize(Roles = $"{Role.Admin},{Role.Moder}")]
        [BanFilter]
        public async Task<IActionResult> AvatarToDefault(int id)
        {
            await _accountService.UpdateImg(id, _imageOptions.AvatarDefault);
            return Ok();
        }

        [HttpPatch("{id}/rename")]
        [Authorize(Roles = $"{Role.Admin},{Role.Moder}")]
        [BanFilter]
        public async Task<IActionResult> RenameAccount(int id, [FromBody] AccountDto accountDto)
        {
            var validator = new AccountDtoAdminUsernameValidator();
            await validator.ValidateAndThrowAsync(accountDto);

            await _accountService.Update(id, User.GetId(), accountDto);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> DeleteAccount(int id) 
        {
            await _accountService.Delete(User.GetId());
            return Ok();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteSelf()
        {
            await _accountService.Delete(User.GetId());
            return Ok();
        }
    }
}