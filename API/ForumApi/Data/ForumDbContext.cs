using ForumApi.Data.ModelConfigurations;
using ForumApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Data
{
    public class ForumDbContext(DbContextOptions<ForumDbContext> options) : DbContext(options)
    {
        public virtual DbSet<Token> Tokens { get; set; } = null!;
        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Section> Sections { get; set; } = null!;
        public virtual DbSet<Forum> Forums { get; set; } = null!;
        public virtual DbSet<Topic> Topics { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<Ban> Bans { get; set; } = null!;
        public virtual DbSet<Models.File> Files { get; set; } = null!;
        public virtual DbSet<Like> Likes { get; set; } = null!;

        public virtual DbSet<Chat> Chats { get; set; } = null!;
        public virtual DbSet<ChatMember> ChatMembers { get; set; } = null!;
        public virtual DbSet<ChatMessage> ChatMessages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            _ = new AccountConfig(builder.Entity<Account>());
            _ = new TokenConfig(builder.Entity<Token>());
            _ = new SectionConfig(builder.Entity<Section>());
            _ = new TopicConfig(builder.Entity<Topic>());
            _ = new ForumConfig(builder.Entity<Forum>());
            _ = new PostConfig(builder.Entity<Post>());
            _ = new BanConfig(builder.Entity<Ban>());
            _ = new FileConfig(builder.Entity<Models.File>());

            _ = new ChatConfig(builder.Entity<Chat>());
            _ = new ChatMemberConfig(builder.Entity<ChatMember>());
            _ = new ChatMessageConfig(builder.Entity<ChatMessage>());

            _ = new LikeConfig(builder.Entity<Like>());

            base.OnModelCreating(builder);
        }
    }
}