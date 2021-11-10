using System.Threading.Tasks;
using Domain.Model;

namespace Application.Rules
{
    public class IdentificationNumberEqualsMatchingRule : RuleContributor
    {
        /// <inheritdoc />
        public override async Task<decimal> MatchAsync(
            MatchingRule rule,
            Person first,
            Person second,
            decimal currentScore,
            NextMatchingRuleDelegate next)
        {
            if (AreIdentificationNumbersPopulatedAndEqual(first.IdentificationNumber, second.IdentificationNumber))
            {
                return Match;
            }

            return await next(currentScore);
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
