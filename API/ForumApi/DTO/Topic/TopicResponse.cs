using ForumApi.DTO.Auth;
using ForumApi.DTO.DPost;

namespace ForumApi.DTO.DTopic;

public class TopicResponse
{
    public TopicDto Topic { get; set; }
    public PostDto Post { get; set; }
    public User Sender { get; set; }
    public List<PostResponse> Posts { get; set; }
}