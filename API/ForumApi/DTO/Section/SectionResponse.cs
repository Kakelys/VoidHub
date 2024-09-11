using ForumApi.DTO.DForum;

namespace ForumApi.DTO.DSection;

public class SectionResponse
{
    public SectionDto Section { get; set; }
    public List<ForumResponse> Forums { get; set; } = [];
}