using System.Threading.Tasks;
using Domain.Model;

namespace Application.Rules
{
    public abstract class RuleContributor : IRuleContributor
    {
        /// <summary>
        /// Represents the value that indicates that there is a match with probability 1 (100%) between the two <see cref="Person"/>.
        /// </summary>
        protected const int Match = 1;

        /// <summary>
        /// Represents the value that indicates that there is no match (0%) between the two <see cref="Person"/>.
        /// </summary>
        protected const int NoMatch = 0;

        /// <inheritdoc />
        public abstract Task<decimal> MatchAsync(
            MatchingRule rule,
            Person first,
            Person second,
            decimal currentScore,
            NextMatchingRuleDelegate next);
    }
}
