using System.Threading.Tasks;
using Domain.Model;

namespace Application.Rules
{
    public interface IRuleContributor
    {
        Task<decimal> MatchAsync(PersonMatchingRule rule, Person first, Person second, decimal currentScore, PersonMatchingRuleDelegate next);
    }
}
