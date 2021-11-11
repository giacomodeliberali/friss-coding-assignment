using System.Threading.Tasks;
using Domain.Model;

namespace Domain.Rules
{
    /// <summary>
    /// The strategy executor that invokes the rules.
    /// </summary>
    public interface IMatchingRuleStrategyExecutor
    {
        /// <summary>
        /// Creates a pipeline for the rules inside the provided strategy and executes them in a pipeline fashion.
        /// </summary>
        /// <param name="strategy">The strategy that contains the rule to be executed.</param>
        /// <param name="first">The first <see cref="Person"/> to compare.</param>
        /// <param name="second">The second <see cref="Person"/> to compare.</param>
        /// <returns>The probability that two provided <see cref="Person"/> have the same identity.</returns>
        Task<decimal> ExecuteAsync(MatchingStrategy strategy, Person first, Person second);
    }
}
