using ForumApi.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumApi.Data.ModelConfigurations
{
    public class TokenConfig
    {
        public TokenConfig(EntityTypeBuilder<Token> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasIndex(t => t.RefreshToken).IsUnique();

            builder.Property(t => t.RefreshToken)
                .IsRequired();
            builder.Property(t => t.ExpiresAt)
                .IsRequired();

            builder.HasOne(t => t.Account)
                .WithMany(a => a.Tokens)
                .HasForeignKey(t => t.AccountId);
        }
    }
}