using ForumApi.Controllers.Filters;
using ForumApi.Data.Models;
using ForumApi.DTO.DChat;
using ForumApi.DTO.Page;
using ForumApi.Services.ChatS.Interfaces;
using ForumApi.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/chats")]
    public class ChatController(IChatService chatService, IMessageService messageService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetChats([FromQuery] Offset offset, [FromQuery] DateTime time)
        {
            return Ok(await chatService.Get(User.GetId(), offset, time));
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
            return Ok(await chatService.CreatePersonal(User.GetId(), dto.TagretId, dto.Content));
        }

        [Authorize]
        [PermissionActionFilterV2<ChatMember>("ChatId", "chatId")]
        [HttpPost("{chatId}/messages")]
        public async Task<IActionResult> SendMessage(int chatId, Message dto)
        {
            return Ok(await messageService.SendMessage(chatId, User.GetId(), dto.Content));
        }

    }
}