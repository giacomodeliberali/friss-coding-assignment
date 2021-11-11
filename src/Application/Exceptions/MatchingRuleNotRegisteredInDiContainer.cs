using Domain.Exceptions;

namespace Application.Exceptions
{
    /// <summary>
    /// Thrown when a rule is not registered into the DI container.
    /// </summary>
    public class MatchingRuleNotRegisteredInDiContainer : BusinessException
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="matchingRuleName">The rule assembly qualified name.</param>
        public MatchingRuleNotRegisteredInDiContainer(string matchingRuleName)
            : base($"MatchingRule '{matchingRuleName}' is not registered in the dependency injection container.")
        {
        }
    }
}
