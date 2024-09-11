namespace ForumApi.Data.Repository.Interfaces;

public interface IRepositoryManager
{
    Lazy<IAccountRepository> Account { get; }
    Lazy<ITokenRepository> Token { get; }
    Lazy<ISectionRepository> Section { get; }
    Lazy<IForumRepository> Forum { get; }
    Lazy<ITopicRepository> Topic { get; }
    Lazy<IPostRepository> Post { get; }
    Lazy<IBanRepository> Ban { get; }
    Lazy<IFileRepository> File { get; }
    Lazy<ILikeRepository> Like { get; }

    Lazy<IChatRepository> Chat { get; }
    Lazy<IChatMemberRepository> ChatMember { get; }
    Lazy<IChatMessageRepository> ChatMessage { get; }

    bool IsInTransaction { get; }

    Task BeginTransaction();
    Task Commit();
    Task Rollback();
    Task Save();
}