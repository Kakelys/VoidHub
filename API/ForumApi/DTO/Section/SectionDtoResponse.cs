using ForumApi.DTO.DForum;

namespace ForumApi.DTO.DSection;

public class SectionDtoResponse
{
    public SectionDto Section { get; set; }
    public List<ForumDto> Forums { get; set; }
}