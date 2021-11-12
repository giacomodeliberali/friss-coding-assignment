using System.Threading.Tasks;

namespace Domain.Rules
{
    /// <summary>
    /// Represents the function that a MatchingRule will be executing when invoked.
    /// <param name="current">The current probability the the previous rules have calculated so far.</param>
    /// <returns>A new probability to be given to the next rule (or the return value if the next function is not invoked)</returns>
    /// </summary>
    public delegate Task<ProbabilitySameIdentity> NextMatchingRuleDelegate(ProbabilitySameIdentity current);
}
