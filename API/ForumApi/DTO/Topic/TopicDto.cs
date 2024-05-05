namespace ForumApi.DTO.DTopic
{
    public class TopicDto
    {
        public int Id { get; set; }
        public int ForumId { get; set; }
        public int AccountId { get; set; }
        public string Title { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsPinned { get; set; }
        public bool IsClosed { get; set; }
        public int PostsCount { get; set; }
    }
}