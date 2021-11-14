using System.Threading.Tasks;
using Domain.Model;

namespace Domain.Rules
{
    /// <summary>
    /// The strategy executor that invokes the rules. This is the entity that is responsible to creating the pipeline
    /// and invoking the rules in the order specified by the strategy.
    /// </summary>
    public interface IMatchingRuleStrategyExecutor
    {
        /// <summary>
        /// Creates a pipeline for executing the rules inside the provided strategy.
        /// </summary>
        /// <param name="strategy">The strategy that contains the <see cref="IMatchingRuleContributor"/> to be executed.</param>
        /// <param name="first">The first <see cref="Person"/> to compare.</param>
        /// <param name="second">The second <see cref="Person"/> to compare.</param>
        /// <returns>The probability that two provided <see cref="Person"/> have the same identity.</returns>
        Task<ProbabilitySameIdentity> ExecuteAsync(MatchingStrategy strategy, Person first, Person second);
    }
}
