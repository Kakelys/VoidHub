using ForumApi.Data.Models;
using ForumApi.DTO.DChat;
using ForumApi.DTO.Page;

namespace ForumApi.Services.ChatS.Interfaces
{
    public interface IMessageService
    {
        Task<MessageResponse> SendMessage(int chatId, int accountId, string message);
        Task<List<MessageResponse>> GetMesages(int chatId, Offset offset, DateTime time);
    }
}