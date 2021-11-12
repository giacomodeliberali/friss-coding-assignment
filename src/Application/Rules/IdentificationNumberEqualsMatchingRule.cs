using System.Threading.Tasks;
using Domain.Model;
using Domain.Rules;
using Microsoft.Extensions.Logging;

namespace Application.Rules
{
    /// <summary>
    /// This rule interrupts the pipeline and return 100% if business identifiers are known and equal.
    /// </summary>
    public class IdentificationNumberEqualsMatchingRule : IRuleContributor
    {
        private readonly ILogger<IdentificationNumberEqualsMatchingRule> _logger;

        /// <summary>
        /// Creates the rule.
        /// </summary>
        public IdentificationNumberEqualsMatchingRule(ILogger<IdentificationNumberEqualsMatchingRule> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<decimal> MatchAsync(
            MatchingRule rule,
            Person first,
            Person second,
            decimal currentProbability,
            NextMatchingRuleDelegate next)
        {
            if (AreIdentificationNumbersPopulatedAndEqual(first.IdentificationNumber, second.IdentificationNumber))
            {
                _logger.LogDebug(
                    "Found identification number match ({First} - {Second}). Returning match",
                    first.IdentificationNumber,
                    second.IdentificationNumber);

                return MatchingProbabilityConstants.Match;
            }

            return await next(currentProbability);
        }

        private bool AreIdentificationNumbersPopulatedAndEqual(string first, string second)
        {
            if (string.IsNullOrEmpty(first) || string.IsNullOrEmpty(second))
            {
                return false;
            }

            return first == second;
        }
    }
}
