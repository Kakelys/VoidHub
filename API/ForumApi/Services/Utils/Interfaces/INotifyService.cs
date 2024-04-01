using ForumApi.DTO.DNotification;

namespace ForumApi.Services.Utils.Interfaces
{
    public interface INotifyService
    {
        Task Notify(int accountId, NotificationBase notif);        
    }
}