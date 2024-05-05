using ForumApi.Data.Models;
using ForumApi.DTO.Auth;

namespace ForumApi.DTO.DPost
{
    public class PostResponse
    {
        public PostDto Post { get; set; } = null!;
        public User Sender { get; set; } = null!;
    }
}