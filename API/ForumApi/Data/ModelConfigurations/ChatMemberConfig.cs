using ForumApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumApi.Data.ModelConfigurations;

public class ChatMemberConfig : IEntityTypeConfiguration<ChatMember>
{

    public void Configure(EntityTypeBuilder<ChatMember> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.Chat)
            .WithMany(c => c.Members)
            .HasForeignKey(c => c.ChatId);

        builder.HasOne(c => c.Account)
            .WithMany(c => c.ChatMembers)
            .HasForeignKey(c => c.AccountId);
    }
}