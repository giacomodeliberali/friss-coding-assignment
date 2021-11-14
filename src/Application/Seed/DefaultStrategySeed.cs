using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Rules;
using Domain.Extensions;
using Domain.Model;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Seed
{
    /// <summary>
    /// Inserts the default strategy if not present (<seealso cref="IDataSeedContributor"/> for why this approach is wrong).
    /// </summary>
    public class DefaultStrategySeed : IDataSeedContributor
    {
        private readonly IMatchingStrategyRepository _matchingStrategyRepository;
        private readonly ILogger<DefaultStrategySeed> _logger;

        /// <summary>
        /// Creates the data seed.
        /// </summary>
        public DefaultStrategySeed(
            IMatchingStrategyRepository matchingStrategyRepository,
            ILogger<DefaultStrategySeed> logger)
        {
            _matchingStrategyRepository = matchingStrategyRepository;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task SeedAsync()
        {
            var strategies = await _matchingStrategyRepository.GetAllAsync();

            if (strategies.Any())
            {
                _logger.LogInformation("Existing strategy found. Skipping seed");
                return;
            }

            var defaultStrategy = MatchingStrategy.Factory.Create(
                "Default",
                "The default strategy with the rules defined in the assignment",
                new List<MatchingRule>()
                {
                    MatchingRule.Factory.Create(
                        typeof(IdentificationNumberEqualsMatchingRule).GetAssemblyQualifiedName(),
                        nameof(IdentificationNumberEqualsMatchingRule),
                        "This rule interrupts the pipeline and return 100% if business identifiers are known and equal.",
                        true,
                        new List<MatchingRuleParameter>()),
                    MatchingRule.Factory.Create(
                        typeof(LastNameMatchingRule).GetAssemblyQualifiedName(),
                        nameof(LastNameMatchingRule),
                        "This rule add 40% if the last names match.",
                        true,
                        new List<MatchingRuleParameter>()),
                    MatchingRule.Factory.Create(
                        typeof(FirstNameMatchingRule).GetAssemblyQualifiedName(),
                        nameof(FirstNameMatchingRule),
                        "This rule add 20% if the first names match or 15% if they are similar.",
                        true,
                        new List<MatchingRuleParameter>()),
                    MatchingRule.Factory.Create(
                        typeof(BirthDateEqualsMatchingRule).GetAssemblyQualifiedName(),
                        nameof(BirthDateEqualsMatchingRule),
                        "This rule add 40% if birth dates match or interrupt the pipeline if both birth dates are known and different.",
                        true,
                        new List<MatchingRuleParameter>()),
                });

            await _matchingStrategyRepository.CreateAsync(defaultStrategy);

            _logger.LogInformation("Seed of Default strategy complete");
        }
    }
}
