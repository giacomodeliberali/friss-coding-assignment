using EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.Configurations
{
    public class PersonMatchingStrategyConfiguration : IEntityTypeConfiguration<PersonMatchingStrategyData>
    {
        public void Configure(EntityTypeBuilder<PersonMatchingStrategyData> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).HasMaxLength(32).IsRequired();
            builder.Property(s => s.Description).HasMaxLength(256).IsRequired();
        }
    }
}
