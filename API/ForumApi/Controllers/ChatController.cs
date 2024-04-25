using ForumApi.Controllers.Filters;
using ForumApi.Data.Models;
using ForumApi.DTO.DChat;
using ForumApi.DTO.DNotification;
using ForumApi.DTO.Utils;
using ForumApi.Hubs;
using ForumApi.Services.ChatS.Interfaces;
using ForumApi.Services.ForumS.Interfaces;
using ForumApi.Services.Utils.Interfaces;
using ForumApi.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/chats")]
    public class ChatController(
        IChatService chatService,
        IMessageService messageService,
        INotifyService notifyService,
        IAccountService accountService) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetChats([FromQuery] Offset offset, [FromQuery] DateTime time)
        {
            return Ok(await chatService.Get(User.GetId(), offset, time));
        }

        [HttpGet("{chatId}")]
        [Authorize]
        [PermissionActionFilterV2<ChatMember>("ChatId", "chatId")]
        public async Task<IActionResult> GetChat(int chatId)
        {
            return Ok(await chatService.Get(chatId));
        }

        [HttpGet("between")]
        [Authorize]
        public async Task<IActionResult> GetBetween([FromQuery] int targetId)
        {
            return Ok(await chatService.Get(User.GetId(), targetId));
        }

        [HttpGet("{chatId}/messages")]
        [Authorize]
        [PermissionActionFilterV2<ChatMember>("ChatId", "chatId")]
        public async Task<IActionResult> GetChatMesages(int chatId, [FromQuery] Offset offset, [FromQuery] DateTime time)
        {
            return Ok(await messageService.GetMesages(chatId, offset, time));
        }

        [HttpPost("personal")]
        [Authorize]
        [BanFilter]
        public async Task<IActionResult> CreatePersonal(Message dto)
        {
            var res = await chatService.CreatePersonal(User.GetId(), dto.TargetId, dto.Content);

            Response.OnCompleted(async () => {
                var user = await accountService.GetUser(User.GetId());
                var notification = new NewMessageNotification
                 {
                    Type = nameof(MessageType.newMessage),
                    Message = res.Item2,
                    Chat = res.Item1,
                    Sender = user
                };

                await notifyService.Notify(dto.TargetId, notification);
            });

            return Ok(res.Item1);
        }

        [HttpPost("{chatId}/messages")]
        [Authorize]
        [PermissionActionFilterV2<ChatMember>("ChatId", "chatId")]
        [BanFilter]
        public async Task<IActionResult> SendMessage(int chatId, Message dto)
        {
            var msgRes = await messageService.SendMessage(chatId, User.GetId(), dto.Content);

            Response.OnCompleted(async () => {
                var chat = await chatService.Get(msgRes.Message.ChatId);
                var notification = new NewMessageNotification
                 {
                    Type = nameof(MessageType.newMessage),
                    Message = msgRes.Message,
                    Chat = chat!.Chat,
                    Sender = msgRes.Sender
                };

                chat.Members.DistinctBy(m => m.Id).ToList().ForEach(m => {
                    notification.AnotherUser = chat.Members.Find(cm => cm.Id != m.Id);

                    notifyService.Notify(m.Id, notification);
                });
            });

            return Ok(msgRes);
        }
    }
}