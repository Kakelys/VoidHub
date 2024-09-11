using ForumApi.Data.Models;
using ForumApi.DTO.DSection;

namespace ForumApi.Services.ForumS.Interfaces;

public interface ISectionService
{
    Task<List<SectionResponse>> GetSections(bool includeHidden = false);
    Task<List<SectionDtoResponse>> GetDtoSections(bool includeHidden = false);
    Task<Section> Create(SectionEdit section);
    Task<Section> Update(int sectionId, SectionEdit section);
    Task Delete(int sectionId);
}