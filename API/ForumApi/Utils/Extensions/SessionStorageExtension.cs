using ForumApi.DTO.Notification;
using ForumApi.Hubs;
using ForumApi.Services.Utils.Interfaces;

namespace ForumApi.Utils.Extensions;

public static class SessionStorageExtension
{
    public static ConnectionsDataNotification GetOnlineStats(this ISessionStorage sessionStorage)
    {
        var users = sessionStorage.Users.ToList();
        return new ConnectionsDataNotification
        {
            Type = MessageType.connectionsData.ToString(),
            TotalCount = sessionStorage.AnonymousContexts.Count() + users.Count,
            Users = users
        };
    }
}