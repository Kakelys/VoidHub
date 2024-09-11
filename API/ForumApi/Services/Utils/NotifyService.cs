using ForumApi.DTO.DNotification;
using ForumApi.Hubs;
using ForumApi.Services.Utils.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ForumApi.Services.Utils;

public class NotifyService(IHubContext<MainHub> hubContext, ISessionStorage sessionStorage) : INotifyService
{
    public async Task Notify(int accountId, NotificationBase notification)
    {
        var connectedUser = sessionStorage.Get(accountId);
        var ctx = hubContext.Clients.Clients(connectedUser.ConnectionIds);

        await ctx.SendAsync("notify", notification);
    }

    public async Task Notify(string contextId, NotificationBase notification)
    {
        var ctx = hubContext.Clients.Client(contextId);

        await ctx.SendAsync("notify", notification);
    }

    public async Task NotifyAll(NotificationBase notification)
    {
        await hubContext.Clients.All.SendAsync("notify", notification);
    }
}