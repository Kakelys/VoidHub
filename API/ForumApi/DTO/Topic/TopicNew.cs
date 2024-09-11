namespace ForumApi.DTO.DTopic;

public class TopicNew : TopicEdit
{
    public string Content { get; set; }

    public List<int> FileIds { get; set; } = [];
}