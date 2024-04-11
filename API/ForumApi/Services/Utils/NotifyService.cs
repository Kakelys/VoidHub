using ForumApi.DTO.DNotification;
using ForumApi.Hubs;
using ForumApi.Services.Utils.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ForumApi.Services.Utils
{
    public class NotifyService(IHubContext<MainHub> hubContext, ISessionStorage sessStorage) : INotifyService
    {
        public async Task Notify(int accountId, NotificationBase notification)
        {
            var contextIds = sessStorage.Get(accountId);
            var ctx = hubContext.Clients.Clients(contextIds);

            await ctx.SendAsync("notify", notification);
        }
    }
}