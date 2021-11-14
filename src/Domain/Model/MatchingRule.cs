using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Exceptions;
using Domain.Extensions;
using Domain.Rules;
using WriteModel;

namespace Domain.Model
{
    /// <summary>
    /// A rule represents some logic that contributes determining the probability that two
    /// <see cref="Person"/> have the same identity.
    ///
    /// Each rule has a type associated that contains the actual code that will run when the rule
    /// is invoked at run time. The actual code is inside the Application layer as it might have dependencies
    /// that would need to be resolved with the DI container.
    ///
    /// A rule type can be any class implementing the <see cref="IMatchingRuleContributor"/> interface.
    /// </summary>
    public class MatchingRule : Entity
    {
        /// <summary>
        /// The rule friendly name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The rule description.
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// Indicates if this rule is enabled ot nor. If the rule is disabled it will be skipped during pipeline invocation.
        /// </summary>
        public bool IsEnabled { get; protected set; }

        /// <summary>
        /// The actual type of this rule, which implements the <see cref="IMatchingRuleContributor"/> interface.
        /// This type will be resolved from DI when invoking this rule and run through the execution pipeline defined in the <see cref="IMatchingRuleStrategyExecutor"/>.
        /// </summary>
        public Type RuleType { get; private set; }

        /// <summary>
        /// The rule parameters (could be empty).
        /// </summary>
        public IReadOnlyCollection<MatchingRuleParameter> Parameters => _parameters;

        /// <summary>
        /// The write model snapshot for the repository.
        /// </summary>
        public MatchingRuleData Snapshot => _snapshot;

        private MatchingRuleData _snapshot;

        private List<MatchingRuleParameter> _parameters;

        private MatchingRule()
        {
        }

        /// <summary>
        /// Returns parameter with the given name if it exists, or it returns the provided fallback value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="defaultValue">The value to use the the parameter does not exist.</param>
        /// <returns>The parameter value or the fallback value.</returns>
        public decimal GetParameterOrDefault(string name, decimal defaultValue = default)
        {
            var parameter = Parameters.SingleOrDefault(p => p.Name == name);

            if (parameter is not null)
            {
                return parameter.Value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Factory used to create validated instances of <see cref="MatchingRule"/>.
        /// </summary>
        public static class Factory
        {
            /// <summary>
            /// Creates a new <see cref="MatchingRule"/>.
            /// </summary>
            /// <param name="ruleTypeAssemblyQualifiedName">The assembly qualified name of the rule type that implements the <see cref="IMatchingRuleContributor"/> interface.</param>
            /// <param name="name">The rule friendly name.</param>
            /// <param name="description">The rule description.</param>
            /// <param name="isEnabled">Indicates if this rule is enabled ot nor. If the rule is disabled it will be skipped during pipeline invocation.</param>
            /// <param name="parameters">The rule parameters.</param>
            /// <returns>The new <see cref="MatchingRule"/></returns>
            /// <exception cref="InvalidRuleTypeException">When the <paramref name="ruleTypeAssemblyQualifiedName"/> does not resolve or does not implement the <see cref="IMatchingRuleContributor"/> interface</exception>
            /// <exception cref="ValidationException">When parameters are not valid.</exception>
            public static MatchingRule Create(
                string ruleTypeAssemblyQualifiedName,
                string name,
                string description,
                bool isEnabled,
                List<MatchingRuleParameter> parameters)
            {
                ruleTypeAssemblyQualifiedName.ThrowIfNullOrEmpty(nameof(ruleTypeAssemblyQualifiedName));
                name.ThrowIfNullOrEmpty(nameof(name));
                description.ThrowIfNullOrEmpty(nameof(description));
                parameters.ThrowIfNull(nameof(parameters));

                if (!MatchingRuleExtensions.TryResolveRuleType(ruleTypeAssemblyQualifiedName, out var ruleType))
                {
                    throw new InvalidRuleTypeException(ruleTypeAssemblyQualifiedName);
                }

                var validRuleParameterNames = ruleType
                    .GetCustomAttributes(typeof(RuleParameterAttribute), inherit: true)
                    .Select(t => ((RuleParameterAttribute)t).ParameterName)
                    .ToList();

                var distinctParameterNames = parameters.Select(p => p.Name).Distinct().ToList();
                if (distinctParameterNames.Count != parameters.Count)
                {
                    throw new DuplicatedParametersException(ruleType.GetAssemblyQualifiedName());
                }

                foreach (var parameter in parameters)
                {
                    if (!validRuleParameterNames.Contains(parameter.Name))
                    {
                        throw new InvalidParameterException(parameter.Name, ruleType.GetAssemblyQualifiedName());
                    }
                }

                return new MatchingRule()
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Description = description,
                    IsEnabled = isEnabled,
                    _parameters = parameters,
                    RuleType = ruleType,
                };
            }

            /// <summary>
            /// Recreates the <see cref="MatchingRule"/> from the database.
            /// </summary>
            /// <param name="snapshot">The write model snapshot.</param>
            /// <param name="parameters">The write model parameters snapshots.</param>
            /// <returns>The <see cref="MatchingRule"/></returns>
            /// <exception cref="InvalidRuleTypeException">When the <paramref name="snapshot"/>.RuleType does not resolve or does not implement the <see cref="IMatchingRuleContributor"/> interface</exception>
            public static MatchingRule FromSnapshot(
                MatchingRuleData snapshot,
                List<MatchingRuleParameterData> parameters)
            {
                if (!MatchingRuleExtensions.TryResolveRuleType(snapshot.RuleTypeAssemblyQualifiedName, out var ruleType))
                {
                    throw new InvalidRuleTypeException(snapshot.RuleTypeAssemblyQualifiedName);
                }

                return new MatchingRule()
                {
                    Id = snapshot.Id,
                    Name = snapshot.Name,
                    Description = snapshot.Description,
                    IsEnabled = snapshot.IsEnabled,
                    RuleType = ruleType,
                    _snapshot = snapshot,
                    _parameters = parameters.Select(MatchingRuleParameter.Factory.FromSnapshot).ToList(),
                };
            }
        }
    }
}
