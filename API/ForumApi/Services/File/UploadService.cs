using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.DFile;
using ForumApi.Options;
using ForumApi.Services.FileS.Interfaces;
using ForumApi.Services.Utils.Interfaces;
using Microsoft.Extensions.Options;

namespace ForumApi.Services.FileS;

public class UploadService(
        IRepositoryManager rep,
        IFileService fileService,
        IImageService imageService,
        IOptions<ImageOptions> imageOptions
    ) : IUploadService
{
    public async Task<FileDto> UploadImage(IFormFile img, FileDto fileDto)
    {
        await rep.BeginTransaction();
        try
        {
            var file = await fileService.Create(fileDto);
            await rep.Save();

            var image = imageService.Load(img);
            imageService.ResizeWithAspect(image, imageOptions.Value.ResizePostWidth, imageOptions.Value.ResizePostHeight);
            await imageService.SaveImage(image, $"{imageOptions.Value.Folder}/{fileDto.Path}");

            await rep.Commit();
            return file;
        }
        catch
        {
            await rep.Rollback();
            throw;
        }
    }

    public async Task<FileDto> UploadVideo(IFormFile video, FileDto fileDto)
    {
        await rep.BeginTransaction();
        try
        {
            var file = await fileService.Create(fileDto);
            await rep.Save();

            var path = $"{imageOptions.Value.Folder}/{fileDto.Path}";
            await using var stream = new FileStream(path, FileMode.Create);
            await video.CopyToAsync(stream);

            await rep.Commit();
            return file;
        }
        catch
        {
            await rep.Rollback();
            throw;
        }
    }

    public async Task<List<FileDto>> DeleteFiles(int[] ids)
    {
        await rep.BeginTransaction();
        try
        {
            var files = new List<FileDto>();
            if (ids.Length > 1)
                files.AddRange(await fileService.Delete(ids));
            else
                files.Add(await fileService.Delete(ids[0]));

            foreach (var file in files)
            {
                var pathToFile = $"{imageOptions.Value.Folder}/{file.Path}";
                if (File.Exists(pathToFile))
                {
                    File.Delete(pathToFile);
                }
                // TODO: Log when not deleted
            }

            await rep.Commit();
            return files;
        }
        catch
        {
            await rep.Rollback();
            throw;
        }
    }
}