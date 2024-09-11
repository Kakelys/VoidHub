using ForumApi.DTO.DTopic;

namespace ForumApi.DTO.DForum;

public class ForumResponse
{
    public ForumDto Forum { get; set; }

    public int PostsCount { get; set; }
    public int TopicsCount { get; set; }

    public TopicLast LastTopic { get; set; }
}