using ForumApi.DTO.Auth;
using ForumApi.DTO.DForum;
using ForumApi.DTO.DPost;

namespace ForumApi.DTO.DTopic;

public class TopicInfoResponse
{
    public User Sender { get; set; }
    public TopicDto Topic { get; set; }
    public ForumDto Forum { get; set; }
    public PostDto Post { get; set; }
}