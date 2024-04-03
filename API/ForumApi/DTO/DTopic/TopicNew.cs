namespace ForumApi.DTO.DTopic
{
    public class TopicNew : TopicEdit
    {
        public string Content { get; set; } = null!;

        public List<int> FileIds { get; set; } = new();
    }
}