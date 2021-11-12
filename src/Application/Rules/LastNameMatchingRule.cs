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
    public class LastNameMatchingRule : IRuleContributor
    {
        private readonly ILogger<LastNameMatchingRule> _logger;

        /// <summary>
        /// The name of the parameter to adjust the probability to add for a last name exact match.
        /// </summary>
        public const string IncreaseProbabilityWhenEqualsLastNames = nameof(IncreaseProbabilityWhenEqualsLastNames);

        /// <summary>
        /// Creates the rule.
        /// </summary>
        /// <param name="logger"></param>
        public LastNameMatchingRule(ILogger<LastNameMatchingRule> logger)
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
            if (first.LastName == second.LastName)
            {
                var increaseProbabilityWhenEqualsLastNames = rule.GetParameterOrDefault(IncreaseProbabilityWhenEqualsLastNames, defaultValue: 0.4m);

                _logger.LogDebug(
                    "Found lastnames match ({First} - {Second}). Adding {Probability}",
                    first.LastName,
                    second.LastName,
                    increaseProbabilityWhenEqualsLastNames);

                currentProbability.AddContributor(rule, increaseProbabilityWhenEqualsLastNames);
            }

            return await next(currentProbability);
        }
    }
}
