using System;
using System.Threading.Tasks;
using Domain.Model;

namespace Domain.Repositories
{
    public interface IMatchingStrategyRepository
    {
        Task<Guid> CreateAsync(MatchingStrategy strategy);

        Task UpdateAsync(MatchingStrategy strategy);

        Task DeleteAsync(MatchingStrategy strategy);

        Task<MatchingStrategy> GetByIdAsync(Guid id);

        Task<MatchingStrategy> GetByNameAsync(string name);
    }
}
