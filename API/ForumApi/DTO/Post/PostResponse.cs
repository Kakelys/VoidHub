using ForumApi.DTO.Auth;

namespace ForumApi.DTO.DPost;

public class PostResponse
{
    public PostDto Post { get; set; }
    public User Sender { get; set; }
}