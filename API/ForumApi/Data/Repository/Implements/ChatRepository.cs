using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;

namespace ForumApi.Data.Repository.Implements
{
    public class ChatRepository(ForumDbContext context) : RepositoryBase<Chat>(context), IChatRepository
    {
    }
}