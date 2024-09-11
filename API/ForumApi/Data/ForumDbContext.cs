using ForumApi.Data.ModelConfigurations;
using ForumApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Data;

public class ForumDbContext(DbContextOptions<ForumDbContext> options) : DbContext(options)
{
    public virtual DbSet<Token> Tokens { get; set; }
    public virtual DbSet<Account> Accounts { get; set; }
    public virtual DbSet<Section> Sections { get; set; }
    public virtual DbSet<Forum> Forums { get; set; }
    public virtual DbSet<Topic> Topics { get; set; }
    public virtual DbSet<Post> Posts { get; set; }
    public virtual DbSet<Ban> Bans { get; set; }
    public virtual DbSet<Models.File> Files { get; set; }
    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Chat> Chats { get; set; }
    public virtual DbSet<ChatMember> ChatMembers { get; set; }
    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = new AccountConfig(modelBuilder.Entity<Account>());
        _ = new TokenConfig(modelBuilder.Entity<Token>());
        _ = new SectionConfig(modelBuilder.Entity<Section>());
        _ = new TopicConfig(modelBuilder.Entity<Topic>());
        _ = new ForumConfig(modelBuilder.Entity<Forum>());
        _ = new PostConfig(modelBuilder.Entity<Post>());
        _ = new BanConfig(modelBuilder.Entity<Ban>());
        _ = new FileConfig(modelBuilder.Entity<Models.File>());

        _ = new ChatConfig(modelBuilder.Entity<Chat>());
        _ = new ChatMemberConfig(modelBuilder.Entity<ChatMember>());
        _ = new ChatMessageConfig(modelBuilder.Entity<ChatMessage>());

        _ = new LikeConfig(modelBuilder.Entity<Like>());

        base.OnModelCreating(modelBuilder);
    }
}