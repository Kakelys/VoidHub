using ForumApi.DTO.Auth;
using ForumApi.DTO.DChat;

namespace ForumApi.DTO.DNotification;

public class NewMessageNotification : NotificationBase
{
    public MessageDto Message { get; set; }
    public User Sender { get; set; }
    public ChatDto Chat { get; set; }
    public User AnotherUser { get; set; }
}