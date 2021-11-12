using Domain.Model;

namespace Domain.Exceptions
{
    /// <summary>
    /// Thrown when creating a <see cref="MatchingRule"/> with duplicate <see cref="MatchingRuleParameter"/>.
    /// </summary>
    public class DuplicatedParametersException : ValidationException
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        public DuplicatedParametersException(string ruleName)
            : base($"Rule '{ruleName}' contains duplicate parameter names")

        {
        }
    }
}
