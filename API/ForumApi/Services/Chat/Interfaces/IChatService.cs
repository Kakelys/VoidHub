using ForumApi.Data.Models;
using ForumApi.DTO.Page;

namespace ForumApi.Services.ChatS.Interfaces
{
    public interface IChatService
    {
        Task<Chat> CreatePersonal(int senderId, int targetId, string message);
        Task<List<Chat>> Get(int accountId, Offset offset, DateTime time);
    }
}