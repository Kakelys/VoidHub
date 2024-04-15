using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumApi.Data.ModelConfigurations
{
    public class FileConfig
    {
        public FileConfig(EntityTypeBuilder<Models.File> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Path)
                .IsRequired();

            builder.Property(f => f.PostId)
                .HasDefaultValue(null);

            builder.HasOne(f => f.Account)
                .WithMany(a => a.UploadedFiles)
                .HasForeignKey(f => f.AccountId);

            builder.HasOne(f => f.Post)
                .WithMany(p => p.Files)
                .HasForeignKey(f => f.PostId)
                .IsRequired(false);

            builder.Property(a => a.CreatedAt)
                .HasDefaultValueSql("timezone('utc', now())");
        }
    }
}