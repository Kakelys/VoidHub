using ForumApi.Data.Repository.Interfaces;

namespace ForumApi.Data.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly ForumDbContext _context;

        public Lazy<IAccountRepository> Account { get; }
        public Lazy<ITokenRepository> Token { get; }

        public Lazy<ISectionRepository> Section { get; }
        public Lazy<IForumRepository> Forum { get; }
        public Lazy<ITopicRepository> Topic { get; }
        public Lazy<IPostRepository> Post { get; }
        public Lazy<IBanRepository> Ban { get; }
        public Lazy<IFileRepository> File { get; }
        public Lazy<ILikeRepository> Like { get; }

        public Lazy<IChatRepository> Chat { get; }
        public Lazy<IChatMemberRepository> ChatMember { get; }
        public Lazy<IChatMessageRepository> ChatMessage { get; }

        public bool IsInTransaction =>
            _context.Database.CurrentTransaction != null;

        public RepositoryManager(
            ForumDbContext context,
            Lazy<IAccountRepository> account, 
            Lazy<ITokenRepository> token,
            Lazy<ISectionRepository> section,
            Lazy<IForumRepository> forum,
            Lazy<ITopicRepository> topic,
            Lazy<IPostRepository> post,
            Lazy<IBanRepository> ban,
            Lazy<IFileRepository> file,
            Lazy<IChatRepository> chat,
            Lazy<IChatMemberRepository> chatMember,
            Lazy<IChatMessageRepository> chatMessage,
            Lazy<ILikeRepository> like
            )
        {
            _context = context;
            Account = account;
            Token = token;

            Section = section;
            Forum = forum;
            Topic = topic;
            Post = post;
            Ban = ban;
            File = file;
            Chat = chat;
            ChatMember = chatMember;
            ChatMessage = chatMessage;
            Like = like;
        }

        public async Task BeginTransaction() => 
            await _context.Database.BeginTransactionAsync();
        
        public async Task Commit() => 
            await _context.Database.CommitTransactionAsync();

        public async Task Rollback() => 
            await _context.Database.RollbackTransactionAsync();

        public async Task Save() => 
            await _context.SaveChangesAsync();
    }
}