using ForumApi.DTO.Auth;
using ForumApi.DTO.DForum;
using ForumApi.DTO.DTopic;

namespace ForumApi.DTO.DPost;

public class PostInfoResponse
{
    public PostDto Post { get; set; }
    public User Sender { get; set; }
    public TopicDto Topic { get; set; }
    public ForumDto Forum { get; set; }
}