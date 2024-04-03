using ForumApi.DTO.DChat;
using ForumApi.DTO.Utils;

namespace ForumApi.Services.ChatS.Interfaces
{
    public interface IChatService
    {
        Task<ChatDto> CreatePersonal(int senderId, int targetId, string message);
        Task<ChatInfo?> Get(int chatId);
        Task<ChatDto?> Get(int accountId, int targetId);
        Task<List<ChatResponse>> Get(int accountId, Offset offset, DateTime time);
    }
}