using System;
using Domain.Model;

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
    }
}
