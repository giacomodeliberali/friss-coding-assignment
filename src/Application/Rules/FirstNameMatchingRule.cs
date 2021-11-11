using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Rules;

namespace Application.Rules
{
    /// <summary>
    /// This rule add 20% if the first names match or 15% if they are similar.
    /// </summary>
    [RuleParameter(IncreaseProbabilityWhenEqualsFirstNames, "The probability to add for a first name exact match.")]
    [RuleParameter(IncreaseProbabilityWhenSimilarFirstNames, "The probability to add for a first name similarity match.")]
    public class FirstNameMatchingRule : IRuleContributor
    {
        // Parameters names
        private const string IncreaseProbabilityWhenEqualsFirstNames = nameof(IncreaseProbabilityWhenEqualsFirstNames);
        private const string IncreaseProbabilityWhenSimilarFirstNames = nameof(IncreaseProbabilityWhenSimilarFirstNames);

        /// <inheritdoc />
        public async Task<decimal> MatchAsync(
            MatchingRule rule,
            Person first,
            Person second,
            decimal currentProbability,
            NextMatchingRuleDelegate next)
        {
            if (first.FirstName == second.FirstName)
            {
                var increaseScorePercentage = rule.GetParameterOrDefault(IncreaseProbabilityWhenEqualsFirstNames, defaultValue: 0.2m);
                return await next(currentProbability + increaseScorePercentage);
            }

            if (AreSimilar(first.FirstName, second.FirstName))
            {
                var increaseScorePercentageSimilarFirstNames = rule.GetParameterOrDefault(IncreaseProbabilityWhenSimilarFirstNames, defaultValue: 0.15m);
                return await next(currentProbability + increaseScorePercentageSimilarFirstNames);
            }

            return await next(currentProbability);
        }

        private bool AreSimilar(string first, string second)
        {
            if (HaveSameInitials(first, second))
            {
                return true;
            }

            if (DiffersForSingleTypo(first, second))
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

        private bool DiffersForSingleTypo(string first, string second)
        {
            var levenshteinDistance = LevenshteinDistance(first, second);

            return levenshteinDistance == 1;
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
