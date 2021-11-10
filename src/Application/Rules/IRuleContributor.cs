using System.Threading.Tasks;
using Domain.Model;

namespace Application.Rules
{
    public interface IRuleContributor
    {
        /// <summary>
        /// Defines the logic that will be invoked when the given MatchingRule is executed. MatchingRules
        /// are executed in a pipeline fashion, where each of the rules can decide to continue or terminate the whole pipeline.
        /// <remarks>To continue the pipeline simply invoke the <paramref name="next"/> delegate providing the score for its execution.</remarks>
        /// <remarks>To interrupt the pipeline just return a value without invoking the <paramref name="next"/> delegate.</remarks>
        /// </summary>
        /// <param name="rule">The rule that could contain parameters.</param>
        /// <param name="first">The first person to compare.</param>
        /// <param name="second">The second person to be compared with.</param>
        /// <param name="currentScore">The score calculated in the pipeline up to this point.</param>
        /// <param name="next">The delegate to the next rule in the pipeline. Invoke it to continue the pipeline or return a value to terminate it.</param>
        /// <returns></returns>
        Task<decimal> MatchAsync(MatchingRule rule, Person first, Person second, decimal currentScore, NextMatchingRuleDelegate next);
    }
}
