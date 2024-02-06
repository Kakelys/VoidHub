using ForumApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumApi.Data.ModelConfigurations
{
    public class SectionConfig
    {
        public SectionConfig(EntityTypeBuilder<Section> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Title)
                .IsRequired();

            builder.Property(s => s.IsHidden)
                .HasDefaultValue(false);
        }
    }
}