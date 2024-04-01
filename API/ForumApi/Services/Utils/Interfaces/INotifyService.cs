using ForumApi.DTO.DNotification;
using ForumApi.DTO.Utils;

namespace ForumApi.Services.Utils.Interfaces
{
    public interface INotifyService
    {
        Task Notify(int accountId, NotificationBase notif);        
    }
}