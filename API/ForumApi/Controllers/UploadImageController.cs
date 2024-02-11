using FluentValidation;
using ForumApi.Controllers.Filters;
using ForumApi.DTO.DFile;
using ForumApi.Options;
using ForumApi.Services;
using ForumApi.Utils.Exceptions;
using ForumApi.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/uploads")]
    public class UploadImageController(
        UploadService uploadService,
        FileService fileService,
        IOptions<ImageOptions> options
    ) : ControllerBase
    {
        private ImageOptions _imageOptions = options.Value;

        [HttpPost("images")]
        [Authorize]
        public async Task<IActionResult> UploadImage(NewFileDto newFileDto)
        {
            var validator = new NewImageValidator(options);
            await validator.ValidateAndThrowAsync(newFileDto);

            var accountId = User.GetId();
            var fileDto = new FileDto
            {
                PostId = newFileDto.PostId,
                AccountId = accountId,
                Path = $"{_imageOptions.Folder}/{_imageOptions.PostImageFolder}/{accountId}{Guid.NewGuid()}",
            };

            return Ok(await uploadService.UploadImage(newFileDto.File, fileDto));
        }

        [HttpDelete("images")]
        [Authorize]
        public async Task<IActionResult> DeteleImages(int[] ids)
        {
            var accountId = User.GetId();
            var files = await fileService.Get(ids);
            if(files.Count(f => f.AccountId == accountId) != files.Count)
                throw new ForbiddenException("You don't have permission to do this action");

            await uploadService.DeleteImages(ids);

            return Ok();
        }

        [HttpDelete("images/{id}")]
        [Authorize]
        [PermissionActionFilter<Data.Models.File>]
        public async Task<IActionResult> DeleteImage(int id)
        {
            await uploadService.DeleteImages([id]);

            return Ok();
        }
    }
}