using ForumApi.Data.Models;
using ForumApi.DTO.DSection;

namespace ForumApi.Services.ForumS.Interfaces
{
    public interface ISectionService
    {
        Task<List<SectionResponse>> GetSections(bool includeHidden = false);
        Task<Section> Create(SectionDto section);
        Task<Section> Update(int sectionId, SectionDto section);
    }
}