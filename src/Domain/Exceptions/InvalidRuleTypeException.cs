using Domain.Rules;

namespace Domain.Exceptions
{
    /// <summary>
    /// Thrown when the rule type if not resolvable or does not implement the <see cref="IRuleContributor"/> interface.
    /// </summary>
    public class InvalidRuleTypeException : ValidationException
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="ruleTypeFullName">The invalid rule type full name.</param>
        public InvalidRuleTypeException(string ruleTypeFullName)
            : base($"Rule type '{ruleTypeFullName}' is not valid (does it implement the IRuleContributor?).")
        {
        }
    }
}
