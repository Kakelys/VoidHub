using ForumApi.DTO.Auth;

namespace ForumApi.DTO.DChat;

public class ChatInfo
{
    public ChatDto Chat { get; set; }
    public List<User> Members { get; set; }
}