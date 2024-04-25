using ForumApi.DTO.DNotification;

namespace ForumApi.Services.Utils.Interfaces
{
    public interface INotifyService
    {
        Task Notify(int accountId, NotificationBase notification);
        Task Notify(string contextId, NotificationBase notification);
        Task NotifyAll(NotificationBase notification);
    }
}