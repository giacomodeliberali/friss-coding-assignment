using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Exceptions;
using Domain.Extensions;
using WriteModel;

namespace Domain.Model
{
    /// <summary>
    /// A strategy is a collection of <see cref="MatchingRule"/> that can be used to calculate the probability
    /// that two <see cref="Person"/> have the same identity.
    /// </summary>
    public class MatchingStrategy : AggregateRoot
    {
        /// <summary>
        /// The strategy name (is unique between all strategies).
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// The strategy description
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// The list of <see cref="MatchingRule"/> that compose this strategy.
        /// </summary>
        public IReadOnlyCollection<MatchingRule> Rules => _rules;

        /// <summary>
        /// The write model snapshot.
        /// </summary>
        public MatchingStrategyData Snapshot => _snapshot;

        private MatchingStrategyData _snapshot;

        private List<MatchingRule> _rules;

        private MatchingStrategy()
        {
        }

        /// <summary>
        /// Updates the properties of this strategy.
        /// </summary>
        /// <param name="name">The new strategy name.</param>
        /// <param name="description">The new strategy description.</param>
        /// <param name="rules">The new strategy rules.</param>
        /// <exception cref="ValidationException">When parameters are invalid.</exception>
        public void Update(string name, string description, List<MatchingRule> rules)
        {
            name.ThrowIfNullOrEmpty(nameof(name));
            description.ThrowIfNullOrEmpty(nameof(description));
            rules.ThrowIfNullOrEmpty(nameof(rules));

            Name = name;
            Description = description;
            _rules = rules;
        }

        /// <summary>
        /// Factory used to create validated instances of <see cref="MatchingStrategy"/>.
        /// </summary>
        public static class Factory
        {
            /// <summary>
            /// Creates a new validated instance of the <see cref="MatchingStrategy"/>.
            /// </summary>
            /// <param name="name">The strategy name.</param>
            /// <param name="description">The strategy description.</param>
            /// <param name="rules">The strategy rules.</param>
            /// <exception cref="ValidationException">When parameters are invalid.</exception>
            /// <returns>A new instance of <see cref="MatchingStrategy"/>.</returns>
            public static MatchingStrategy Create(
                string name,
                string description,
                List<MatchingRule> rules)
            {
                name.ThrowIfNullOrEmpty(nameof(name));
                description.ThrowIfNullOrEmpty(nameof(description));
                rules.ThrowIfNullOrEmpty(nameof(rules));

                return new MatchingStrategy()
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Description = description,
                    _rules = rules,
                };
            }

            /// <summary>
            /// Recreates the <see cref="MatchingStrategy"/> from the database.
            /// </summary>
            /// <param name="snapshot">The write model snapshot.</param>
            /// <param name="rulesSnapshot">The write model rules snapshot.</param>
            /// <param name="parametersSnapshots">The write model parameters snapshot.</param>
            /// <returns>A new instance of <see cref="MatchingStrategy"/>.</returns>
            public static MatchingStrategy FromSnapshot(
                MatchingStrategyData snapshot,
                List<MatchingRuleData> rulesSnapshot,
                List<MatchingRuleParameterData> parametersSnapshots)
            {
                return new MatchingStrategy()
                {
                    Id = snapshot.Id,
                    Name = snapshot.Name,
                    Description = snapshot.Description,
                    _snapshot = snapshot,
                    _rules = rulesSnapshot.Select(r =>
                    {
                        return MatchingRule.Factory.FromSnapshot(r, parametersSnapshots.Where(p => p.RuleId == r.Id).ToList());
                    }).ToList(),
                };
            }
        }
    }
}
