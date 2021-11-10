using System.Threading.Tasks;
using Domain.Model;

namespace Application.Rules
{
    public class IdentificationNumberEqualsMatchingRule : RuleContributor
    {
        public override async Task<decimal> MatchAsync(
            PersonMatchingRule rule,
            Person first,
            Person second,
            decimal currentScore,
            PersonMatchingRuleDelegate next)
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
