using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;

namespace ForumApi.Data.Repository.Implements
{
    public class ChatMessageRepository(ForumDbContext context) : RepositoryBase<ChatMessage>(context), IChatMessageRepository
    {
    }
}