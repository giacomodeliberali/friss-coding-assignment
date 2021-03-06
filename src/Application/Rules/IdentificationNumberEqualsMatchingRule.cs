using System.Threading.Tasks;
using Domain.Model;
using Domain.Rules;
using Microsoft.Extensions.Logging;

namespace Application.Rules
{
    /// <summary>
    /// This rule interrupts the pipeline and return 100% if business identifiers are known and equal.
    /// </summary>
    public class IdentificationNumberEqualsMatchingRule : IMatchingRuleContributor
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
        public async Task<ProbabilitySameIdentity> MatchAsync(
            MatchingRule rule,
            Person first,
            Person second,
            ProbabilitySameIdentity currentProbability,
            NextMatchingRuleDelegate next)
        {
            if (AreIdentificationNumbersPopulatedAndEqual(first.IdentificationNumber, second.IdentificationNumber))
            {
                _logger.LogDebug("Found identification number match. Returning 100%");
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
