using System.Threading.Tasks;
using Domain.Model;

namespace Domain.Rules
{
    public class IdentificationNumberEqualsMatchingRule : PersonMatchingRule
    {
        public override async Task<decimal> Match(Person first, Person second, decimal score, PersonMatchingRuleDelegate next)
        {
            if (AreIdentificationNumbersPopulatedAndEqual(first.IdentificationNumber, second.IdentificationNumber))
            {
                return 1;
            }

            return await next(score);
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
