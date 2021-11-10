using System.Threading.Tasks;
using Domain.Model;

namespace Application.Rules
{
    public interface IPersonMatchingRuleStrategyExecutor
    {
        Task<decimal> ExecuteAsync(PersonMatchingStrategy strategy, Person first, Person second);
    }
}
