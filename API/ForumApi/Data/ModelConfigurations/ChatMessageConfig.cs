using ForumApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumApi.Data.ModelConfigurations;

public class ChatMessageConfig : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())");

        builder.Property(c => c.DeletetAt)
            .HasDefaultValue(null);
        builder.Property(c => c.ModifiedAt)
            .HasDefaultValue(null);

        builder.HasOne(c => c.Member)
            .WithMany(cm => cm.Messages)
            .HasForeignKey(c => c.ChatMemberId);

        builder.HasOne(c => c.Chat)
            .WithMany(c => c.Messages)
            .HasForeignKey(c => c.ChatId);
    }
}