using System.Threading.Tasks;
using Domain.Model;

namespace Application.Rules
{
    public abstract class RuleContributor : IRuleContributor
    {
        protected const int Match = 1;
        protected const int NoMatch = 0;

        public abstract Task<decimal> MatchAsync(
            PersonMatchingRule rule,
            Person first,
            Person second,
            decimal currentScore,
            PersonMatchingRuleDelegate next);
    }
}
