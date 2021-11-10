using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WriteModel;

namespace EntityFrameworkCore.Configurations
{
    public class MatchingStrategyConfiguration : IEntityTypeConfiguration<MatchingStrategyData>
    {
        public void Configure(EntityTypeBuilder<MatchingStrategyData> builder)
        {
            builder.ToTable("Rules.Strategies");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).HasMaxLength(32).IsRequired();
            builder.Property(s => s.Description).HasMaxLength(256).IsRequired();
        }
    }
}
