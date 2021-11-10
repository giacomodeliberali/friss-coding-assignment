using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WriteModel;

namespace EntityFrameworkCore.Configurations
{
    public class MatchingRuleConfiguration : IEntityTypeConfiguration<MatchingRuleData>
    {
        public void Configure(EntityTypeBuilder<MatchingRuleData> builder)
        {
            builder.ToTable("Rules.Rules");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name).HasMaxLength(32).IsRequired();
            builder.Property(r => r.Description).HasMaxLength(256).IsRequired();
            builder.Property(r => r.IsEnabled);
            builder.Property(r => r.Order).IsRequired();
            builder.Property(r => r.StrategyId).IsRequired();
            builder.Property(r => r.RuleTypeAssemblyQualifiedName).HasMaxLength(256).IsRequired();
        }
    }
}
