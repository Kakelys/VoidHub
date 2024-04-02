using ForumApi.DTO.Auth;

namespace ForumApi.DTO.DChat
{
    public class ChatInfo
    {
        public ChatDto Chat { get; set; } = null!;
        public List<User> Members { get; set; } = null!;        
    }
}