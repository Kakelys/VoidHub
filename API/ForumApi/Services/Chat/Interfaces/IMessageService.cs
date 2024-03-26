using ForumApi.Data.Models;
using ForumApi.DTO.Page;

namespace ForumApi.Services.ChatS.Interfaces
{
    public interface IMessageService
    {
        Task<ChatMessage> SendMessage(int chatId, int accountId, string message);
        Task<List<ChatMessage>> GetMesages(int chatId, Offset offset, DateTime time);
    }
}