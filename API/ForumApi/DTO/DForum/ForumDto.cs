namespace ForumApi.DTO.DForum
{
    public class ForumDto
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string Title { get; set; } = null!;
        public DateTime? DeletedAt { get; set; }
        public bool IsClosed { get; set; }        
    }
}