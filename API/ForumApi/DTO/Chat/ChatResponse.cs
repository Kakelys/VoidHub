using ForumApi.DTO.Auth;

namespace ForumApi.DTO.DChat;

public class ChatResponse
{
    public ChatDto Chat { get; set; }
    public MessageDto LastMessage { get; set; }
    public User Sender { get; set; }
    public User AnotherUser { get; set; }
}