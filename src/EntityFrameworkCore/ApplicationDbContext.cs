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

        /// <summary>
        /// The table that contains all the <see cref="MatchingStrategies"/>.
        /// </summary>
        public DbSet<MatchingStrategyData> MatchingStrategies { get; set; }

        /// <summary>
        /// The table that contains all the <see cref="MatchingRule"/>.
        /// </summary>
        public DbSet<MatchingRuleData> MatchingRules { get; set; }

        /// <summary>
        /// The table that contains all the <see cref="MatchingRuleParameter"/>.
        /// </summary>
        public DbSet<MatchingRuleParameterData> MatchingRulesParameters { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add custom configurations
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
            modelBuilder.ApplyConfiguration(new MatchingStrategyConfiguration());
            modelBuilder.ApplyConfiguration(new MatchingRuleConfiguration());
            modelBuilder.ApplyConfiguration(new MatchingRuleParameterConfiguration());
        }
    }
}
