using System.Threading.Tasks;
using Domain.Model;

namespace Application.Rules
{
    public interface IMatchingRuleStrategyExecutor
    {
        Task<decimal> ExecuteAsync(MatchingStrategy strategy, Person first, Person second);
    }
}
