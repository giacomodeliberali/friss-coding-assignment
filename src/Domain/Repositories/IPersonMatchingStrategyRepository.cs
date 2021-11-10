using System;
using System.Threading.Tasks;
using Domain.Model;

namespace Domain.Repositories
{
    public interface IPersonMatchingStrategyRepository
    {
        Task<Guid> CreateAsync(PersonMatchingStrategy strategy);

        Task UpdateAsync(PersonMatchingStrategy strategy);

        Task DeleteAsync(PersonMatchingStrategy strategy);

        Task<PersonMatchingStrategy> GetByIdAsync(Guid id);

        Task<PersonMatchingStrategy> GetByNameAsync(string name);
    }
}
