using ForumApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumApi.Data.ModelConfigurations
{
    public class ForumConfig
    {
        public ForumConfig(EntityTypeBuilder<Forum> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Title)
                .IsRequired();

            builder.Property(f => f.IsClosed)
                .HasDefaultValue(false);

            builder.Property(f => f.DeletedAt)
                .HasDefaultValue(null);

            builder.Property(f => f.ImagePath)
                    .HasDefaultValue("forum_default.png");

            builder.HasOne(f => f.Section)
                .WithMany(s => s.Forums)
                .HasForeignKey(f => f.SectionId);
        }
    }
}