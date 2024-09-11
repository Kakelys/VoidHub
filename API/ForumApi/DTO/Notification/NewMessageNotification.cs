using ForumApi.DTO.Auth;
using ForumApi.DTO.DChat;

namespace ForumApi.DTO.DNotification;

public class NewMessageNotification : NotificationBase
{
    public MessageDto Message { get; set; } = null!;
    public User Sender { get; set; } = null!;
    public ChatDto Chat { get; set; } = null!;
    public User AnotherUser { get; set; }
}