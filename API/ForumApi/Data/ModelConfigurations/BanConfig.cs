using ForumApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumApi.Data.ModelConfigurations
{
    public class BanConfig
    {
        public BanConfig(EntityTypeBuilder<Ban> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("timezone('utc', now())");

            builder.Property(b => b.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("timezone('utc', now())");

            builder.Property(b => b.ExpiresAt)
                .IsRequired();

            builder.Property(b => b.Reason)
                .IsRequired();

            builder.Property(b => b.IsActive);

            builder.HasOne(b => b.Account)
                .WithMany(a => a.RecievedBans)
                .HasForeignKey(b => b.AccountId);

            builder.HasOne(b => b.Moderator)
                .WithMany(a => a.GivenBans)
                .HasForeignKey(b => b.ModeratorId);

            builder.HasOne(b => b.UpdatedBy)
                .WithMany(a => a.UpdatedBans)
                .HasForeignKey(b => b.UpdatedById);
        }
    }
}