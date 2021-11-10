using EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<PersonData>
    {
        public void Configure(EntityTypeBuilder<PersonData> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.FirstName).HasMaxLength(32).IsRequired();
            builder.Property(p => p.LastName).HasMaxLength(32).IsRequired();
            builder.Property(p => p.BirthDate);
            builder.Property(p => p.IdentificationNumber).HasMaxLength(64);
        }
    }
}
