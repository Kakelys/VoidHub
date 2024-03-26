using ForumApi.Data.ModelConfigurations;
using ForumApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Data
{
    public class ForumDbContext : DbContext
    {
        public virtual DbSet<Token> Tokens { get; set; } = null!;
        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Section> Sections { get; set; } = null!;
        public virtual DbSet<Forum> Forums { get; set; } = null!;
        public virtual DbSet<Topic> Topics { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<Ban> Bans { get; set; } = null!;
        public virtual DbSet<Models.File> Files { get; set; } = null!;

        public virtual DbSet<Chat> Chats { get; set; } = null!;
        public virtual DbSet<ChatMember> ChatMembers { get; set; } = null!;
        public virtual DbSet<ChatMessage> ChatMessages { get; set; } = null!;

        public ForumDbContext(DbContextOptions<ForumDbContext> options) 
            : base(options)
        {}
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            new AccountConfig(builder.Entity<Account>());
            new TokenConfig(builder.Entity<Token>());
            new SectionConfig(builder.Entity<Section>());
            new TopicConfig(builder.Entity<Topic>());
            new ForumConfig(builder.Entity<Forum>());
            new PostConfig(builder.Entity<Post>());
            new BanConfig(builder.Entity<Ban>()) ;
            new FileConfig(builder.Entity<Models.File>());

            new ChatConfig(builder.Entity<Chat>());
            new ChatMemberConfig(builder.Entity<ChatMember>());
            new ChatMessageConfig(builder.Entity<ChatMessage>());

            base.OnModelCreating(builder);
        }
    }
}