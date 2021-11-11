using System;
using Domain.Model;
using Domain.Rules;

namespace Domain.Extensions
{
    /// <summary>
    /// Helper extensions for the <see cref="MatchingRule"/>.
    /// </summary>
    public static class MatchingRuleExtensions
    {
        /// <summary>
        /// Returns the type full name and assembly without the assembly key information.
        /// <example>
        /// <code>
        /// // "Application.Rules.LastNameMatchingRule, Application"
        /// var ruleType = typeof(LastNameMatchingRule).GetAssemblyQualifiedName();
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetAssemblyQualifiedName(this Type type)
        {
            return $"{type.FullName}, {type.Assembly.GetName().Name}";
        }

        /// <summary>
        /// Returns true if the provided type is a valid rule type (a class that implements the <see cref="IRuleContributor"/> interface).
        /// </summary>
        /// <param name="ruleType">The type to check.</param>
        /// <returns>True if the type inherits from <see cref="IRuleContributor"/>, false otherwise.</returns>
        public static bool IsValidRuleType(this Type ruleType)
        {
            return ruleType is not null && typeof(IRuleContributor).IsAssignableFrom(ruleType);
        }

        /// <summary>
        /// Treis to resolve the rule type from the assembly qualified name.
        /// </summary>
        /// <param name="assemblyQualifiedName">The assembly qualified name for the rule type.</param>
        /// <param name="ruleType">If the rule type is resolved correctly it is populated.</param>
        /// <returns>True if the type is resolved correctly, false otherwise.</returns>
        public static bool TryResolveRuleType(string assemblyQualifiedName, out Type ruleType)
        {
            try
            {
                ruleType = Type.GetType(assemblyQualifiedName);
                return ruleType.IsValidRuleType();
            }
            catch
            {
                ruleType = null;
                return false;
            }
        }
    }
}
