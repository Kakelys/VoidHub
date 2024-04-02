using ForumApi.Data.Models;
using ForumApi.DTO.Auth;

namespace ForumApi.DTO.DChat
{
    public class ChatResponse
    {
        public ChatDto Chat { get; set; } = null!;
        public MessageDto LastMessage { get; set; } = null!;
        public User Sender { get; set; } = null!;
        public User? AnotherUser { get; set; } = null!;
    }
}