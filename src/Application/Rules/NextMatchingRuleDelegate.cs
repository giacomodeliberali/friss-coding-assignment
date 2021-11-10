using System.Threading.Tasks;

namespace Application.Rules
{
    /// <summary>
    /// Represents the function that a MatchingRule will be executing when invoked.
    /// <param name="score">The current score the the previous rules have calculated so far.</param>
    /// <returns>A new score to be given to the next rule (or the return value if the next function is not invoked)</returns>
    /// </summary>
    public delegate Task<decimal> NextMatchingRuleDelegate(decimal score);
}
