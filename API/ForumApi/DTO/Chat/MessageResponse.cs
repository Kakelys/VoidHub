using ForumApi.DTO.Auth;

namespace ForumApi.DTO.DChat
{
    public class MessageResponse
    {
        public MessageDto Message { get; set; } = null!;
        public User Sender { get; set; } = null!;
    }
}