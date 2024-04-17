using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;
using ForumApi.Controllers.Filters;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.DFile;
using ForumApi.Options;
using ForumApi.Services.FileS.Interfaces;
using ForumApi.Utils.Exceptions;
using ForumApi.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/uploads/images")]
    public class UploadImageController(
        IUploadService uploadService,
        IFileService fileService,
        IOptions<ImageOptions> options,
        IJsonStringLocalizer locale,
        Lazy<IFileRepository> fileRep
    ) : ControllerBase
    {
        private readonly ImageOptions _imageOptions = options.Value;

        [HttpPost]
        [Authorize]
        [BanFilter]
        public async Task<IActionResult> UploadImage(NewFileDto newFileDto)
        {
            var validator = new NewImageValidator(options, locale);
            await validator.ValidateAndThrowAsync(newFileDto);

            var accountId = User.GetId();
            var fileDto = new FileDto
            {
                PostId = newFileDto.PostId,
                AccountId = accountId,
                Path = $"{_imageOptions.PostImageFolder}/{accountId}{Guid.NewGuid()}{_imageOptions.FileType}",
            };

            if(fileRep.Value.FindByCondition(f => f.AccountId == accountId).Count() > options.Value.LimitPerAccount)
                throw new BadRequestException(locale["errors.image-limit-exceeded"]);

            return Ok(await uploadService.UploadImage(newFileDto!.File!, fileDto));
        }

        [HttpDelete]
        [Authorize]
        [BanFilter]
        public async Task<IActionResult> DeteleImages([FromQuery] int[] ids)
        {
            if(ids.Length == 0)
                throw new BadRequestException(locale["errors.file-ids-not-provided"]);

            var accountId = User.GetId();

            // check permission for delition
            var files = await fileService.Get(ids);
            if(files.Count(f => f.AccountId == accountId) != files.Count)
                throw new ForbiddenException(locale["errors.no-permission"]);

            await uploadService.DeleteImages(ids);

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        [PermissionActionFilter<Data.Models.File>]
        [BanFilter]
        public async Task<IActionResult> DeleteImage(int id)
        {
            await uploadService.DeleteImages([id]);

            return Ok();
        }
    }
}