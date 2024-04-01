using ForumApi.Services.Utils.Interfaces;
using ForumApi.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ForumApi.Hubs
{
    public class MainHub(ISessionStorage sessionStorage) : Hub
    {
        public async Task SomeMethod()
        {
            Console.WriteLine("Some method");
        }

        #region conenction overrides

        [Authorize]
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("session: on connect");
            if(Context.User?.Identity?.IsAuthenticated == false)
            {
                Context.Abort();
                return;
            }

            var userId = Context.User.GetId();
            sessionStorage.Add(Context.ConnectionId, userId);
            Console.WriteLine($"add user: {userId} {Context.ConnectionId}");
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("session: on disconnect");
            sessionStorage.Remove(Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        #endregion
    }
}