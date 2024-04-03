using ForumApi.Controllers.Filters;
using ForumApi.Data.Models;
using ForumApi.DTO.DChat;
using ForumApi.DTO.DNotification;
using ForumApi.DTO.Utils;
using ForumApi.Services.ChatS.Interfaces;
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
        INotifyService notifyService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetChats([FromQuery] Offset offset, [FromQuery] DateTime time)
        {
            return Ok(await chatService.Get(User.GetId(), offset, time));
        }

        [Authorize]
        [PermissionActionFilterV2<ChatMember>("ChatId", "chatId")]
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChat(int chatId)
        {
            return Ok(await chatService.Get(chatId));
        }

        [Authorize]
        [HttpGet("between")]
        public async Task<IActionResult> GetBetween([FromQuery] int targetId)
        {
            return Ok(await chatService.Get(User.GetId(), targetId));
        }

        [Authorize]
        [PermissionActionFilterV2<ChatMember>("ChatId", "chatId")]
        [HttpGet("{chatId}/messages")]
        public async Task<IActionResult> GetChatMesages(int chatId, [FromQuery] Offset offset, [FromQuery] DateTime time)
        {
            return Ok(await messageService.GetMesages(chatId, offset, time));
        }
        
        [Authorize]
        [HttpPost("personal")]
        public async Task<IActionResult> CreatePersonal(Message dto)
        {
            return Ok(await chatService.CreatePersonal(User.GetId(), dto.TargetId, dto.Content));
        }

        [Authorize]
        [PermissionActionFilterV2<ChatMember>("ChatId", "chatId")]
        [HttpPost("{chatId}/messages")]
        public async Task<IActionResult> SendMessage(int chatId, Message dto)
        {
            var msgRes = await messageService.SendMessage(chatId, User.GetId(), dto.Content);

            Response.OnCompleted(async () => {
                var chat = await chatService.Get(msgRes.Message.ChatId);
                var notification = new NewMessageNotification
                 {
                    Type = "newMessage",
                    Message = msgRes.Message,
                    Chat = chat.Chat,
                    Sender = msgRes.Sender
                };

                //chat.Members.Where(m => m.Id != msgRes.Sender.Id)
                chat.Members.ToList().ForEach(m => {
                    if(!chat.Chat.IsGroup)
                        notification.AnotherUser = chat.Members.FirstOrDefault(cm => cm.Id != m.Id);

                    notifyService.Notify(m.Id, notification);
                });
            });

            return Ok(msgRes);
        }

    }
}