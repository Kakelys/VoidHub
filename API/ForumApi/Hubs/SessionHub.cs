using ForumApi.Services.Socket.Interfaces;
using ForumApi.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using SignalR.Modules;

namespace ForumApi.Hubs
{
    public class SessionHub (ISessionStorage sessionStorage) : ModuleHub
    {

        [Authorize]
        public override async Task OnConnectedAsync()
        {
            if(Context.User?.Identity?.IsAuthenticated == false)
            {
                Context.Abort();
                return;
            }

            var userId = Context.User.GetId();
            sessionStorage.Add(Context.ConnectionId, userId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            sessionStorage.Remove(Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}