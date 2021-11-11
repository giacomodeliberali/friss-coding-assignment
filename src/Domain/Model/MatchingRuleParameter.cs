using System;
using WriteModel;

namespace Domain.Model
{
    /// <summary>
    /// Represent a single parameter for a <see cref="MatchingRule"/>. Each rule can have its own parameters
    /// that allow the customization of the rule's runtime behaviour.
    /// </summary>
    public class MatchingRuleParameter : Entity
    {
        /// <summary>
        /// The parameter name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The parameter value.
        /// </summary>
        public decimal Value { get; set; }


        private MatchingRuleParameterData _snapshot;

        /// <summary>
        /// The write model snapshot.
        /// </summary>
        public MatchingRuleParameterData Snapshot => _snapshot;

        private MatchingRuleParameter()
        {
        }

        /// <summary>
        /// Factory used to create validated instances of <see cref="MatchingRuleParameter"/>.
        /// </summary>
        public static class Factory
        {
            /// <summary>
            /// Creates a new validated instance of the <see cref="MatchingRuleParameter"/>.
            /// </summary>
            /// <param name="name">The parameter name.</param>
            /// <param name="value">The parameter value.</param>
            /// <returns>The <see cref="MatchingRuleParameter"/>.</returns>
            public static MatchingRuleParameter Create(string name, decimal value)
            {
                return new MatchingRuleParameter()
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Value = value,
                };
            }

            /// <summary>
            /// Recreates the <see cref="MatchingRuleParameter"/> from the database.
            /// </summary>
            /// <param name="snapshot">The write model snapshot.</param>
            /// <returns>The <see cref="MatchingRuleParameter"/>.</returns>
            public static MatchingRuleParameter FromSnapshot(MatchingRuleParameterData snapshot)
            {
                return new MatchingRuleParameter()
                {
                    Id = snapshot.Id,
                    Name = snapshot.Name,
                    Value = snapshot.Value,
                    _snapshot = snapshot,
                };
            }
        }
    }
}
