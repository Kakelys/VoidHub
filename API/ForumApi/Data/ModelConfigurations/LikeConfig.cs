using ForumApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumApi.Data.ModelConfigurations;

public class LikeConfig : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.HasKey(l => new { l.AccountId, l.PostId });

        builder.HasOne(l => l.Account)
            .WithMany(a => a.Likes)
            .HasForeignKey(l => l.AccountId);

        builder.HasOne(l => l.Post)
            .WithMany(a => a.Likes)
            .HasForeignKey(l => l.PostId);
    }
}