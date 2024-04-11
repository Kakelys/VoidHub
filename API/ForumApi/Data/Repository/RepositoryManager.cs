using ForumApi.Data.Repository.Interfaces;

namespace ForumApi.Data.Repository
{
    public class RepositoryManager(
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
            ) : IRepositoryManager
    {
        public Lazy<IAccountRepository> Account { get; } = account;
        public Lazy<ITokenRepository> Token { get; } = token;

        public Lazy<ISectionRepository> Section { get; } = section;
        public Lazy<IForumRepository> Forum { get; } = forum;
        public Lazy<ITopicRepository> Topic { get; } = topic;
        public Lazy<IPostRepository> Post { get; } = post;
        public Lazy<IBanRepository> Ban { get; } = ban;
        public Lazy<IFileRepository> File { get; } = file;
        public Lazy<ILikeRepository> Like { get; } = like;

        public Lazy<IChatRepository> Chat { get; } = chat;
        public Lazy<IChatMemberRepository> ChatMember { get; } = chatMember;
        public Lazy<IChatMessageRepository> ChatMessage { get; } = chatMessage;

        public bool IsInTransaction =>
            context.Database.CurrentTransaction != null;

        public async Task BeginTransaction() =>
            await context.Database.BeginTransactionAsync();

        public async Task Commit() =>
            await context.Database.CommitTransactionAsync();

        public async Task Rollback() =>
            await context.Database.RollbackTransactionAsync();

        public async Task Save() =>
            await context.SaveChangesAsync();
    }
}