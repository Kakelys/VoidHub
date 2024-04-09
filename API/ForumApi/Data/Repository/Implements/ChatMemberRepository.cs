using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;

namespace ForumApi.Data.Repository.Implements
{
    public class ChatMemberRepository(ForumDbContext context) : RepositoryBase<ChatMember>(context), IChatMemberRepository
    {
    }
}