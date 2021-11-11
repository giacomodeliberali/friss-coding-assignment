using System;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Rules;

namespace Application.Rules
{
    /// <summary>
    /// This rule add 40% if birth dates match or interrupt the pipeline if both birth dates are known and different.
    /// </summary>
    [RuleParameter(IncreaseProbabilityWhenBirthDateMatches, "The probability to add when birthdates match.")]
    public class BirthDateEqualsMatchingRule : IRuleContributor
    {
        private const string IncreaseProbabilityWhenBirthDateMatches = nameof(IncreaseProbabilityWhenBirthDateMatches);

        /// <inheritdoc />
        public async Task<decimal> MatchAsync(
            MatchingRule rule,
            Person first,
            Person second,
            decimal currentProbability,
            NextMatchingRuleDelegate next)
        {
            if (AreBirthDatesPopulated(first.BirthDate, second.BirthDate))
            {
                // do not consider time when comparing dates
                if (first.BirthDate!.Value.Date == second.BirthDate!.Value.Date)
                {
                    var increaseProbabilityWhenBirthDatesMatch = rule.GetParameterOrDefault(IncreaseProbabilityWhenBirthDateMatches, defaultValue: 0.4m);
                    return await next(currentProbability + increaseProbabilityWhenBirthDatesMatch);
                }

                return MatchingProbabilityConstants.NoMatch;
            }

            return await next(currentProbability);
        }

        private bool AreBirthDatesPopulated(DateTime? first, DateTime? second)
        {
            return first.HasValue && second.HasValue;
        }
    }
}
