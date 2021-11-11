using Domain.Model;

namespace Domain.Exceptions
{
    /// <summary>
    /// Thrown when the <see cref="MatchingRuleParameter"/> is not available in the selected <see cref="MatchingRule"/>.
    /// </summary>
    public class InvalidParameterException : ValidationException
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        public InvalidParameterException(string parameterName, string ruleName)
            : base($"Rule '{ruleName}' does not have a parameter named '{parameterName}'")
        {
        }
    }
}
