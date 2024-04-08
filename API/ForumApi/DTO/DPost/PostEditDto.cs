namespace ForumApi.DTO.DPost
{
    public class PostEditDto
    {
        public int TopicId { get; set; }
        public string Content { get; set; } = null!;
        public int? AncestorId { get; set; } = null;

        public List<int> FileIds { get; set; } = new();
    }
}