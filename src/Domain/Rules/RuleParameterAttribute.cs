using System;

namespace Domain.Rules
{
    /// <summary>
    /// Allows to specify the parameters that a rule permits.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RuleParameterAttribute : Attribute
    {
        /// <summary>
        /// The parameter name.
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        /// The parameter description.
        /// </summary>
        public string ParameterDescription { get; }

        /// <summary>
        /// Specifies that this <see cref="IMatchingRuleContributor"/> allow the following parameter.
        /// </summary>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="parameterDescription">The parameter description.</param>
        public RuleParameterAttribute(string parameterName, string parameterDescription)
        {
            ParameterName = parameterName;
            ParameterDescription = parameterDescription;
        }
    }
}
