using ForumApi.DTO.DFile;

namespace ForumApi.Services.FileS.Interfaces
{
    public interface IFileService
    {
        Task<List<FileDto>> Get(int? postId);
        Task<List<FileDto>> Get(int[] ids);
        /// <summary>
        /// where postId == null
        /// </summary>
        Task<List<FileDto>> GetInvalid();
        Task<FileDto> Create(FileDto file);
        Task Update(int[] ids, int? postId);
        Task<List<FileDto>> Delete(int[] ids);
        Task<FileDto> Delete(int id);
    }
}