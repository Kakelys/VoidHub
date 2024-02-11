using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.DFile;
using ForumApi.Services.Interfaces;

namespace ForumApi.Services
{
    public class UploadService(
            IRepositoryManager rep,
            IFileService fileService,
            IImageService imageService
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
                await imageService.SaveImage(image, fileDto.Path);

                await rep.Commit();
                return file;
            } 
            catch 
            {
                await rep.Rollback();
                throw;
            }
        }

        public async Task<List<FileDto>> DeleteImages(int[] ids)
        {
            await rep.BeginTransaction();
            try 
            {
                var files = await fileService.Delete(ids);
                await rep.Save();
                
                foreach(var file in files)
                {
                    if(File.Exists(file.Path))
                    {
                        File.Delete(file.Path);
                    }
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
}