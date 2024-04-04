using ForumApi.DTO.DForum;

namespace ForumApi.DTO.DSection
{
    public class SectionDtoResponse
    {
        public SectionDto Section { get; set; } = null!;
        public List<ForumDto> Forums { get; set; } = null!;    
    }
}