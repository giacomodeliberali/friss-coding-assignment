using System.Threading.Tasks;
using Domain.Model;

namespace Application.Rules
{

    public class LastNameMatchingRule : RuleContributor
    {
        public const string IncreaseScorePercentageParameterName = "IncreaseScorePercentage";

        public override async Task<decimal> MatchAsync(
            PersonMatchingRule rule,
            Person first,
            Person second,
            decimal currentScore,
            PersonMatchingRuleDelegate next)
        {
            if (first.LastName == second.LastName)
            {
                var increaseScorePercentage = rule.GetParameterOrDefault(IncreaseScorePercentageParameterName, defaultValue: 0.4m);
                return await next(currentScore + increaseScorePercentage);
            }

            return await next(currentScore);
        }
    }
}
