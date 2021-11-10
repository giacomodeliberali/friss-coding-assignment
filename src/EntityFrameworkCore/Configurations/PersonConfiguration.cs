using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WriteModel;

namespace EntityFrameworkCore.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<PersonData>
    {
        public void Configure(EntityTypeBuilder<PersonData> builder)
        {
            builder.ToTable("People");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.FirstName).HasMaxLength(32).IsRequired();
            builder.Property(p => p.LastName).HasMaxLength(32).IsRequired();
            builder.Property(p => p.BirthDate);
            builder.Property(p => p.IdentificationNumber).HasMaxLength(64);
        }
    }
}
