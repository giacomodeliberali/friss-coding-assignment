using System;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Rules
{
    public class BirthDateEqualsMatchingRule : RuleContributor
    {
        public override async Task<decimal> MatchAsync(
            PersonMatchingRule rule,
            Person first,
            Person second,
            decimal currentScore,
            PersonMatchingRuleDelegate next)
        {
            if (AreBirthDatesPopulated(first.BirthDate, second.BirthDate))
            {
                // do not consider time when comparing dates
                if (first.BirthDate!.Value.Date == second.BirthDate!.Value.Date)
                {
                    return await next(currentScore + 0.4m);
                }

                return NoMatch;
            }

            return await next(currentScore);
        }

        private bool AreBirthDatesPopulated(DateTime? first, DateTime? second)
        {
            return first.HasValue && second.HasValue;
        }
    }
}
