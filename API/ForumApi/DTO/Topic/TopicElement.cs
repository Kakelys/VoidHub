using ForumApi.Data.Models;
using ForumApi.DTO.Auth;
using ForumApi.DTO.DPost;

namespace ForumApi.DTO.DTopic;

public class TopicElement : Topic
{
    public new User Author { get; set; } = null!;
    public LastPost LastPost { get; set; }

    public TopicElement() { }

    public TopicElement(Topic topic)
    {
        Id = topic.Id;
        ForumId = topic.ForumId;
        Title = topic.Title;
        CreatedAt = topic.CreatedAt;
        DeletedAt = topic.DeletedAt;
        IsClosed = topic.IsClosed;
        IsPinned = topic.IsPinned;
        PostsCount = topic.PostsCount;
    }
}