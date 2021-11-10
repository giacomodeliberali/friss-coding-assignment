using EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.Configurations
{
    public class PersonMatchingRuleConfiguration : IEntityTypeConfiguration<PersonMatchingRuleData>
    {
        public void Configure(EntityTypeBuilder<PersonMatchingRuleData> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name).HasMaxLength(32).IsRequired();
            builder.Property(r => r.Description).HasMaxLength(256).IsRequired();
            builder.Property(r => r.Order).IsRequired();
            builder.Property(r => r.StrategyId).IsRequired();
            builder.Property(r => r.RuleTypeAssemblyQualifiedName).HasMaxLength(256).IsRequired();
        }
    }
}
