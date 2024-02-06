using ForumApi.Data.Models;
using ForumApi.DTO.Auth;

namespace ForumApi.DTO.DPost
{
    public class PostResponse : Post
    {

        public new User Author { get; set; } = null!;

        public PostResponse() {}

        public PostResponse(Post post) 
        {
            Id = post.Id;
            AncestorId = post.Id;
            TopicId = post.TopicId;
            Content = post.Content;
            CreatedAt = post.CreatedAt;
            DeletedAt = post.DeletedAt;
            CommentsCount = post.CommentsCount;
        }
    }
}