using ForumApi.DTO.Auth;
using ForumApi.DTO.DNotification;

namespace ForumApi.DTO.Notification
{
    public class ConnectionsDataNotification : NotificationBase
    {
        public int TotalCount { get; set; }
        public List<User> Users { get; set; } = null!;
    }
}