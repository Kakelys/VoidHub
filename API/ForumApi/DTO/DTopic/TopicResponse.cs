using ForumApi.Data.Models;
using ForumApi.DTO.DPost;

namespace ForumApi.DTO.DTopic
{
    public class TopicResponse : Topic
    {
        public int PostsCount { get; set; }        
        public int CommentsCount { get; set; }

        public PostResponse Post { get; set; } = null!;
        public new List<PostResponse> Posts { get; set; } = null!;

        public TopicResponse() {}

        public TopicResponse(Topic topic)
        {
            Id = topic.Id;
            ForumId = topic.ForumId;
            Title = topic.Title;
            CreatedAt = topic.CreatedAt;
            DeletedAt = topic.DeletedAt;
            IsClosed = topic.IsClosed;
            IsPinned = topic.IsPinned;
        }
    }
}