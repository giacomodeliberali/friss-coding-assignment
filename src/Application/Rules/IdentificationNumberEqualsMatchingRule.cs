using System.Threading.Tasks;
using Domain.Model;
using Domain.Rules;

namespace Application.Rules
{
    /// <summary>
    /// This rule interrupts the pipeline and return 100% if business identifiers are known and equal.
    /// </summary>
    public class IdentificationNumberEqualsMatchingRule : IRuleContributor
    {
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
