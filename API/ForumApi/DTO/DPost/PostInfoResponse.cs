using ForumApi.DTO.Auth;
using ForumApi.DTO.DTopic;

namespace ForumApi.DTO.DPost
{
    public class PostInfoResponse
    {
        public PostDto Post { get; set; } = null!;
        public User Sender { get; set; } = null!;
        public TopicDto Topic { get; set; } = null!;
    }
}