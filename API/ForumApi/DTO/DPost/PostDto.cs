namespace ForumApi.DTO.DPost
{
    public class PostDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int TopicId { get; set; }
        public int? AncestorId { get; set; }
        public int CommentsCount { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}