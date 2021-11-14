using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Rules;
using Microsoft.Extensions.Logging;

namespace Application.Rules
{
    /// <summary>
    /// This rule add 20% if the first names match or 15% if they are similar.
    /// </summary>
    [RuleParameter(IncreaseProbabilityWhenEqualsFirstNames, "The probability to add for a first name exact match.")]
    [RuleParameter(IncreaseProbabilityWhenSimilarFirstNames, "The probability to add for a first name similarity match.")]
    public class FirstNameMatchingMatchingRule : IMatchingRuleContributor
    {
        private readonly ILogger<FirstNameMatchingMatchingRule> _logger;

        /// <summary>
        /// The name of the parameter to adjust the probability to add for a first name exact match.
        /// </summary>
        public const string IncreaseProbabilityWhenEqualsFirstNames = nameof(IncreaseProbabilityWhenEqualsFirstNames);

        /// <summary>
        /// The name of the parameter to adjust the probability to add for a first name similarity match.
        /// </summary>
        public const string IncreaseProbabilityWhenSimilarFirstNames = nameof(IncreaseProbabilityWhenSimilarFirstNames);

        /// <summary>
        /// Creates the rule.
        /// </summary>
        public FirstNameMatchingMatchingRule(ILogger<FirstNameMatchingMatchingRule> logger)
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
            if (first.FirstName == second.FirstName)
            {
                var increaseProbabilityWhenEqualsFirstNames = rule.GetParameterOrDefault(IncreaseProbabilityWhenEqualsFirstNames, defaultValue: 0.2m);

                _logger.LogDebug(
                    "Found firstnames match ({First} - {Second}). Adding {Probability}",
                    first.FirstName,
                    second.FirstName,
                    increaseProbabilityWhenEqualsFirstNames);

                currentProbability.AddContributor(rule, increaseProbabilityWhenEqualsFirstNames);
            }
            else if (AreSimilar(first.FirstName, second.FirstName))
            {
                var increaseScorePercentageSimilarFirstNames = rule.GetParameterOrDefault(IncreaseProbabilityWhenSimilarFirstNames, defaultValue: 0.15m);

                _logger.LogDebug(
                    "Found firstnames similarity ({First} - {Second}). Adding {Probability}",
                    first.FirstName,
                    second.FirstName,
                    increaseScorePercentageSimilarFirstNames);

                currentProbability.AddContributor(rule, increaseScorePercentageSimilarFirstNames);
            }

            return await next(currentProbability);
        }

        private bool AreSimilar(string first, string second)
        {
            if (HaveSameInitials(first, second))
            {
                return true;
            }

            if (AreDiminutive(first, second))
            {
                return true;
            }

            return false;
        }

        private bool AreDiminutive(string first, string second)
        {
            // GDL super bad heuristic!
            var levenshteinDistance = LevenshteinDistance(first, second);

            return levenshteinDistance <= 3;
        }

        private bool HaveSameInitials(string firstName, string firstName2)
        {
            if (firstName.ElementAt(0) + "." == firstName2.Substring(0, 2))
            {
                return true;
            }

            if (firstName2.ElementAt(0) + "." == firstName.Substring(0, 2))
            {
                return true;
            }

            return false;
        }


        private int LevenshteinDistance(string first, string second)
        {
            var n = first.Length;
            var m = second.Length;
            var d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (var i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (var j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (var i = 1; i <= n; i++)
            {
                //Step 4
                for (var j = 1; j <= m; j++)
                {
                    // Step 5
                    var cost = (second[j - 1] == first[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }
}
