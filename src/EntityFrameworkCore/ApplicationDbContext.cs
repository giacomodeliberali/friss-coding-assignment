using Domain.Model;
using EntityFrameworkCore.Configurations;
using Microsoft.EntityFrameworkCore;
using WriteModel;

namespace EntityFrameworkCore
{
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// The table that contains all created <see cref="Person"/>.
        /// </summary>
        public DbSet<PersonData> People { get; set; }

        public DbSet<PersonMatchingStrategyData> PersonMatchingStrategies { get; set; }

        public DbSet<PersonMatchingRuleData> PersonMatchingRules { get; set; }

        public DbSet<PersonMatchingRuleParameterData> PersonMatchingRulesParameters { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add custom configurations
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
            modelBuilder.ApplyConfiguration(new PersonMatchingStrategyConfiguration());
            modelBuilder.ApplyConfiguration(new PersonMatchingRuleConfiguration());
            modelBuilder.ApplyConfiguration(new PersonMatchingRuleParameterConfiguration());
        }
    }
}
