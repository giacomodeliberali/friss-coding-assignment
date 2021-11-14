using System.Threading.Tasks;
using Domain.Model;

namespace Domain.Rules
{
    /// <summary>
    /// Defines the logic that will be invoked when the given MatchingRule is executed.
    /// The idea is that the rules might have dependencies (eg. logger or other external services accessed by network or IO).
    /// Since rules could have those deps, I decided not to put them in the Domain project (it only references the WriteModel).
    /// By having the matching logic in the Application we can exploit the DI to resolve external dependencies.
    /// </summary>
    public interface IMatchingRuleContributor
    {
        /// <summary>
        /// Defines the logic that will be invoked when the given MatchingRule is executed. MatchingRules
        /// are executed in a pipeline fashion, where each of the rules can decide to continue or terminate the whole pipeline.
        /// <remarks>To continue the pipeline simply invoke the <paramref name="next"/> delegate providing the score for its execution.
        /// You can modify the <paramref name="currentProbability"/> prior to give it the the <paramref name="next"/> handler.</remarks>
        /// <remarks>To interrupt the pipeline just return a value without invoking the <paramref name="next"/> delegate.</remarks>
        /// </summary>
        /// <param name="rule">The rule that could contain parameters.</param>
        /// <param name="first">The first person to compare.</param>
        /// <param name="second">The second person to be compared with.</param>
        /// <param name="currentProbability">The probability calculated in the pipeline up to this point.</param>
        /// <param name="next">The delegate to the next rule in the pipeline. Invoke it to continue the pipeline or return a value to terminate it.</param>
        /// <returns>The updated probability (possibly updated).</returns>
        Task<ProbabilitySameIdentity> MatchAsync(MatchingRule rule, Person first, Person second, ProbabilitySameIdentity currentProbability, NextMatchingRuleDelegate next);
    }
}
