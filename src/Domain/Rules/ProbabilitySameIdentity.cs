using System;
using System.Collections.Generic;
using Domain.Exceptions;
using Domain.Extensions;
using Domain.Model;

namespace Domain.Rules
{
    /// <summary>
    /// Represent the probability that two <see cref="Person"/> have the same identity.
    /// </summary>
    public class ProbabilitySameIdentity
    {
        /// <summary>
        /// Represents the value that indicates that there is a match with probability 1 (100%) between the two <see cref="Person"/>.
        /// </summary>
        public const decimal Match = 1;

        /// <summary>
        /// Represents the value that indicates that there is no match (0%) between the two <see cref="Person"/>.
        /// </summary>
        public const decimal NoMatch = 0;

        private readonly List<Contributor> _contributors;

        /// <summary>
        /// The probability that two <see cref="Person"/> have the same identity (range 0-1).
        /// </summary>
        public decimal Probability { get; private set; }

        /// <summary>
        /// The list of rules that contributed to the final probability.
        /// </summary>
        public IReadOnlyCollection<Contributor> Contributors => _contributors.AsReadOnly();

        /// <summary>
        /// Initialize probability with the optional initial value (default to 0%)
        /// </summary>
        /// <param name="initialProbability">The initial probability (default 0)</param>
        /// <exception cref="ValidationException">When the provided <paramref name="initialProbability"/> is not in the 0-1 range.</exception>
        public ProbabilitySameIdentity(decimal initialProbability = NoMatch)
        {
            if (initialProbability < 0)
            {
                throw new ValidationException("Probability cannot be less than zero.");
            }

            if (initialProbability > 1)
            {
                throw new ValidationException("Probability cannot be more than one.");
            }

            Probability = initialProbability;
            _contributors = new List<Contributor>();
        }

        /// <summary>
        /// Adds the specified <paramref name="value"/> to the current probability value.
        /// The final probability always remains in the 0-1 range.
        /// </summary>
        /// <param name="rule">The rule that contributed.</param>
        /// <param name="value">The value to add to the current probability.</param>
        public void AddContributor(MatchingRule rule, decimal value)
        {
            // ensure range 0-1
            Probability = Math.Max(
                Math.Min(
                    Probability + value,
                    Match),
                NoMatch);

            _contributors.Add(new Contributor(rule, value));
        }

        /// <summary>
        /// Sets the probability to 0.
        /// </summary>
        /// <param name="rule">The rule that moved the probability to zero.</param>
        /// <returns>The new probability set to 0.</returns>
        public ProbabilitySameIdentity SetNoMatch(MatchingRule rule)
        {
            Probability = NoMatch;
            _contributors.Add(new Contributor(rule, NoMatch));
            return this;
        }

        /// <summary>
        /// Sets the probability to 1.
        /// </summary>
        /// <param name="rule">The rule that moved the probability to one.</param>
        /// <returns>The new probability set to 1.</returns>
        public ProbabilitySameIdentity SetMatch(MatchingRule rule)
        {
            Probability = Match;
            _contributors.Add(new Contributor(rule, Match));
            return this;
        }

        /// <summary>
        /// Returns true then the current probability is 1 (100%).
        /// </summary>
        /// <returns>True if the current probability is 1.</returns>
        public bool IsMatch()
        {
            return Probability == Match;
        }

        /// <summary>
        /// Represents the rule that helped in the calculation of the current probability.
        /// </summary>
        public record Contributor
        {
            /// <summary>
            /// The rule name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// The rule description.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// The actual rule type.
            /// </summary>
            public string RuleType { get; set; }

            /// <summary>
            /// The value with which the rule contributed to the calculation.
            /// </summary>
            public decimal Value { get; set; }

            /// <summary>
            /// Creates a new contributor.
            /// </summary>
            /// <param name="matchingRule">The matching rule that contributed.</param>
            /// <param name="value">The value with which the rule contributed to the calculation.</param>
            public Contributor(MatchingRule matchingRule, decimal value)
            {
                Name = matchingRule.Name;
                Description = matchingRule.Description;
                RuleType = matchingRule.RuleType.GetAssemblyQualifiedName();
                Value = value;
            }
        }
    }
}
