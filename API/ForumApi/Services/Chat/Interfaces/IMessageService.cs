using ForumApi.DTO.DChat;
using ForumApi.DTO.Utils;

namespace ForumApi.Services.ChatS.Interfaces;

public interface IMessageService
{
    Task<MessageResponse> SendMessage(int chatId, int accountId, string message);
    Task<List<MessageResponse>> GetMessages(int chatId, Offset offset, DateTime time);
}