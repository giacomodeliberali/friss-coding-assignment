using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WriteModel;

namespace EntityFrameworkCore.Configurations
{
    public class MatchingRuleParameterConfiguration : IEntityTypeConfiguration<MatchingRuleParameterData>
    {
        public void Configure(EntityTypeBuilder<MatchingRuleParameterData> builder)
        {
            builder.ToTable("Rules.RuleParameters");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).HasMaxLength(32).IsRequired();
            builder.Property(s => s.Value).IsRequired();
            builder.Property(s => s.RuleId).IsRequired();
        }
    }
}
