using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;

namespace ForumApi.Data.Repository.Implements
{
    public class ChatMemberRepository : RepositoryBase<ChatMember>, IChatMemberRepository
    {
        public ChatMemberRepository(ForumDbContext context) : base(context)
        {
        }
    }
}