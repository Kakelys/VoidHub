using AutoMapper;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.Services.Utils.Interfaces;
using ForumApi.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ForumApi.DTO.Utils;
using ForumApi.DTO.Auth;
using ForumApi.DTO.Notification;

namespace ForumApi.Hubs
{
    public class MainHub(
        ISessionStorage sessionStorage,
        IAccountRepository accRep,
        IMapper mapper,
        INotifyService notifyService) : Hub
    {
        #region connection stuff

        [Authorize]
        [AllowAnonymous]
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("session: on connect");
            if(Context.User?.Identity?.IsAuthenticated == false)
            {
                // add to anonymous
                sessionStorage.AddAnonymous(Context.ConnectionId);
                await base.OnConnectedAsync();
                return;
            }

            var userId = Context!.User!.GetId();
            var user = accRep.FindById(userId).First();

            sessionStorage.Add(mapper.Map<User>(user), Context.ConnectionId, userId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("session: on disconnect");
            var userId = sessionStorage.Get(Context.ConnectionId);
            if(userId != -1)
            {
                sessionStorage.Remove(Context.ConnectionId);
            }
            else
            {
                sessionStorage.RemoveAnonymous(Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task GetConnectionsData()
        {
            await notifyService.Notify(Context.ConnectionId, sessionStorage.GetOnlineStats());
        }

        #endregion
    }
}