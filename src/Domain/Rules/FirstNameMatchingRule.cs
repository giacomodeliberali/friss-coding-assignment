using System.Threading.Tasks;
using Domain.Model;

namespace Domain.Rules
{

    public class FirstNameMatchingRule : PersonMatchingRule
    {
        public const string IncreaseScorePercentageParameterName = "IncreaseScorePercentage";

        public override async Task<decimal> Match(Person first, Person second, decimal currentScore, PersonMatchingRuleDelegate next)
        {
            if (first.FirstName == second.FirstName)
            {
                var increaseScorePercentage = GetParameterOrDefault(IncreaseScorePercentageParameterName, defaultValue: 0.4m);
                return await next(currentScore + increaseScorePercentage);
            }

            return await next(currentScore);
        }
    }
}
