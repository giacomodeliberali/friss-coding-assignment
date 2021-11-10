using System.Threading.Tasks;

namespace Application.Rules
{
    public delegate Task<decimal> PersonMatchingRuleDelegate(decimal score);
}
