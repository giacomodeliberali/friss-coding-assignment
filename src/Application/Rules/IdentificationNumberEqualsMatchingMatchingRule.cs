using System.Threading.Tasks;
using Domain.Model;
using Domain.Rules;
using Microsoft.Extensions.Logging;

namespace Application.Rules
{
    /// <summary>
    /// This rule interrupts the pipeline and return 100% if business identifiers are known and equal.
    /// </summary>
    public class IdentificationNumberEqualsMatchingMatchingRule : IMatchingRuleContributor
    {
        private readonly ILogger<IdentificationNumberEqualsMatchingMatchingRule> _logger;

        /// <summary>
        /// Creates the rule.
        /// </summary>
        public IdentificationNumberEqualsMatchingMatchingRule(ILogger<IdentificationNumberEqualsMatchingMatchingRule> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ProbabilitySameIdentity> MatchAsync(
            MatchingRule rule,
            Person first,
            Person second,
            ProbabilitySameIdentity currentProbability,
            NextMatchingRuleDelegate next)
        {
            if (AreIdentificationNumbersPopulatedAndEqual(first.IdentificationNumber, second.IdentificationNumber))
            {
                _logger.LogDebug(
                    "Found identification number match ({First} - {Second}). Returning match",
                    first.IdentificationNumber,
                    second.IdentificationNumber);

                return currentProbability.SetMatch(rule);
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
