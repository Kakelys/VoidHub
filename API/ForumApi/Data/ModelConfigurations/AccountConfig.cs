using ForumApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumApi.Data.ModelConfigurations
{
    public class AccountConfig
    {
        public AccountConfig(EntityTypeBuilder<Account> builder)
        {
                builder.HasKey(a => a.Id);

                builder.HasIndex(a => a.LoginName).IsUnique();
                builder.HasIndex(a => a.Email).IsUnique();

                builder.Property(a => a.Username)
                    .IsRequired();
                builder.Property(a => a.LoginName)
                    .IsRequired();
                builder.Property(a => a.Email)
                    .IsRequired();
                builder.Property(a => a.PasswordHash)
                    .IsRequired();
                builder.Property(a => a.LastLoggedAt)
                    .HasDefaultValueSql("timezone('utc', now())");
                builder.Property(a => a.CreatedAt)
                    .HasDefaultValueSql("timezone('utc', now())");
                builder.Property(a => a.AvatarPath)
                    .HasDefaultValue("default.png");

                builder.Property(a => a.DeletedAt)
                    .HasDefaultValue(null);
        }
    }
}