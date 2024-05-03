using ForumApi.DTO.Auth;
using ForumApi.DTO.DForum;
using ForumApi.DTO.DPost;

namespace ForumApi.DTO.DTopic
{
    public class TopicInfoResponse
    {
        public User Sender { get; set; } = null!;
        public TopicDto Topic { get; set; } = null!;
        public ForumDto Forum { get; set; } = null!;
        public PostDto Post { get; set; } = null!;
    }
}