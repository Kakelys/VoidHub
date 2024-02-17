using ForumApi.DTO.DFile;

namespace ForumApi.Services.Interfaces
{
    public interface IUploadService
    {
        Task<FileDto> UploadImage(IFormFile img, FileDto fileDto);
        Task<List<FileDto>> DeleteImages(int[] ids);
    }
}