using System.Threading.Tasks;
using Domain.Model;
using Domain.Rules;
using Microsoft.Extensions.Logging;

namespace Application.Rules
{
    /// <summary>
    /// This rule add 40% if the last names match.
    /// </summary>
    [RuleParameter(IncreaseProbabilityWhenEqualsLastNames, "The probability to add for a last name exact match.")]
    public class LastNameMatchingMatchingRule : IMatchingRuleContributor
    {
        private readonly ILogger<LastNameMatchingMatchingRule> _logger;

        /// <summary>
        /// The name of the parameter to adjust the probability to add for a last name exact match.
        /// </summary>
        public const string IncreaseProbabilityWhenEqualsLastNames = nameof(IncreaseProbabilityWhenEqualsLastNames);

        /// <summary>
        /// Creates the rule.
        /// </summary>
        public LastNameMatchingMatchingRule(ILogger<LastNameMatchingMatchingRule> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ProbabilitySameIdentity> MatchAsync(
            MatchingRule rule,
            Person first,
            Person second,
            ProbabilitySameIdentity currentProbability,
            NextMatchingRuleDelegate next)
        {
            // Note: we could have a rule parameter that allow us to specify the type of comparison (eg. IgnoreCase)
            if (first.LastName == second.LastName)
            {
                var increaseProbabilityWhenEqualsLastNames = rule.GetParameterOrDefault(IncreaseProbabilityWhenEqualsLastNames, defaultValue: 0.4m);

                _logger.LogDebug(
                    "Found lastnames match. Adding {Probability}",
                    increaseProbabilityWhenEqualsLastNames);

                currentProbability.AddContributor(rule, increaseProbabilityWhenEqualsLastNames);
            }

            return await next(currentProbability);
        }
    }
}
