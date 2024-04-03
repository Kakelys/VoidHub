namespace ForumApi.DTO.DSection
{
    public class SectionDto
    {
        public int Id { get; set; }
        public int OrderPosition { get; set; }
        public string Title { get; set; } = null!;
        public bool IsHidden { get; set; }        
    }
}