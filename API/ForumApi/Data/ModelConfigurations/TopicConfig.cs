using ForumApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumApi.Data.ModelConfigurations;

public class TopicConfig : IEntityTypeConfiguration<Topic>
{
    public void Configure(EntityTypeBuilder<Topic> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())");

        builder.Property(t => t.IsClosed)
            .HasDefaultValue(false);

        builder.Property(t => t.IsPinned)
            .HasDefaultValue(false);

        builder.Property(t => t.DeletedAt)
            .HasDefaultValue(null);

        builder.Property(t => t.PostsCount)
            .HasDefaultValue(0);

        builder.HasGeneratedTsVectorColumn(
                t => t.SearchVector,
                "english",
                t => new { t.Title }
            )
            .HasIndex(t => t.SearchVector)
            .HasMethod("GIN");

        builder.HasOne(t => t.Author)
            .WithMany(a => a.Topics)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(t => t.Forum)
            .WithMany(f => f.Topics)
            .HasForeignKey(t => t.ForumId);
    }
}