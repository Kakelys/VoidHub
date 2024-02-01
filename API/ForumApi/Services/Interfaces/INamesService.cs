using ForumApi.DTO.DName;

namespace ForumApi.Services.Interfaces
{
    public interface INamesService
    {
        Task<List<Name>> GetForums(bool includeHidden = false);
        Task<List<Name>> GetSections();
    }
}