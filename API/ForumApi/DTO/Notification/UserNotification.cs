using ForumApi.DTO.Auth;
using ForumApi.DTO.DNotification;

namespace ForumApi.DTO.Notification
{
    public class UserNotification : NotificationBase
    {
        public User? User { get; set; }
    }
}