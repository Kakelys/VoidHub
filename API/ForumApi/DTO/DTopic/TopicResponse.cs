using ForumApi.DTO.Auth;
using ForumApi.DTO.DPost;

namespace ForumApi.DTO.DTopic
{
    public class TopicResponse
    { 
        public TopicDto Topic { get; set; }= null!;
        public PostDto Post { get; set; } = null!;
        public User Sender { get; set; }= null!;
        public List<PostResponse> Posts { get; set; } = null!;
    }
}