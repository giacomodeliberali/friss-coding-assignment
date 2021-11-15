using System;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Rules;
using Microsoft.Extensions.Logging;

namespace Application.Rules
{
    /// <summary>
    /// This rule add 40% if birth dates match or interrupt the pipeline if both birth dates are known and different.
    /// </summary>
    [RuleParameter(IncreaseProbabilityWhenBirthDateMatches, "The probability to add when birthdates match.")]
    public class BirthDateEqualsMatchingRule : IMatchingRuleContributor
    {
        private readonly ILogger<BirthDateEqualsMatchingRule> _logger;

        /// <summary>
        /// The name of the parameter to adjust the probability to add when birthdates match.
        /// </summary>
        public const string IncreaseProbabilityWhenBirthDateMatches = nameof(IncreaseProbabilityWhenBirthDateMatches);

        /// <summary>
        /// Creates the rule.
        /// </summary>
        public BirthDateEqualsMatchingRule(ILogger<BirthDateEqualsMatchingRule> logger)
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
            if (AreBirthDatesPopulated(first.BirthDate, second.BirthDate))
            {
                // do not consider time when comparing dates
                if (first.BirthDate!.Value.Date != second.BirthDate!.Value.Date)
                {
                    _logger.LogDebug("Birth dates are populated and different. Returning 0%");
                    return currentProbability.SetNoMatch(rule);
                }

                var increaseProbabilityWhenBirthDatesMatch = rule.GetParameterOrDefault(IncreaseProbabilityWhenBirthDateMatches, defaultValue: 0.4m);

                _logger.LogDebug(
                    "Found birth dates match. Adding {Probability}",
                    increaseProbabilityWhenBirthDatesMatch);

                currentProbability.AddContributor(rule, increaseProbabilityWhenBirthDatesMatch);
            }

            return await next(currentProbability);
        }

        private bool AreBirthDatesPopulated(DateTime? first, DateTime? second)
        {
            return first.HasValue && second.HasValue;
        }
    }
}
