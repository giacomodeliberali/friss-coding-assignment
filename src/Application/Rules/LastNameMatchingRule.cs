using System.Threading.Tasks;
using Domain.Model;
using Domain.Rules;

namespace Application.Rules
{
    /// <summary>
    /// This rule add 40% if the last names match.
    /// </summary>
    [RuleParameter(IncreaseProbabilityWhenEqualsLastNames, "The probability to add for a last name exact match.")]
    public class LastNameMatchingRule : IRuleContributor
    {
        /// <summary>
        /// The name of the parameter to adjust the probability to add for a last name exact match.
        /// </summary>
        public const string IncreaseProbabilityWhenEqualsLastNames = nameof(IncreaseProbabilityWhenEqualsLastNames);

        /// <inheritdoc />
        public async Task<decimal> MatchAsync(
            MatchingRule rule,
            Person first,
            Person second,
            decimal currentProbability,
            NextMatchingRuleDelegate next)
        {
            if (first.LastName == second.LastName)
            {
                var increaseProbabilityWhenEqualsLastNames = rule.GetParameterOrDefault(IncreaseProbabilityWhenEqualsLastNames, defaultValue: 0.4m);
                return await next(currentProbability + increaseProbabilityWhenEqualsLastNames);
            }

            return await next(currentProbability);
        }
    }
}
