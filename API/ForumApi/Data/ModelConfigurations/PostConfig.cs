using ForumApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumApi.Data.ModelConfigurations
{
    public class PostConfig
    {
        public PostConfig(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.AncestorId)
                .HasDefaultValue(null);

            builder.Property(p => p.CommentsCount)
                .HasDefaultValue(0);
            builder.Property(p => p.LikesCount)
                .HasDefaultValue(0);

            builder.Property(p => p.Content)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("timezone('utc', now())");

            builder.Property(p => p.DeletedAt)
                .HasDefaultValue(null);

            builder.HasGeneratedTsVectorColumn(
                    p => p.SearchVector,
                    "english",
                    p => new { p.Content }
                )
                .HasIndex(t => t.SearchVector)
                .HasMethod("GIN");

            builder.HasOne(p => p.Topic)
                .WithMany(t => t.Posts)
                .HasForeignKey(p => p.TopicId);

            builder.HasOne(p => p.Author)
                .WithMany(a => a.Posts)
                .HasForeignKey(p => p.AccountId);

            builder.HasMany(p => p.Comments)
                .WithOne(p => p.Ancestor)
                .HasForeignKey(p => p.AncestorId)
                .IsRequired(false);
        }
    }
}