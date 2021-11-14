using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Model;

namespace Domain.Repositories
{
    /// <summary>
    /// The repository for the <see cref="MatchingStrategy"/> aggregate root.
    /// Note: this should be implemented with a generic IRepository(Entity)
    /// </summary>
    public interface IMatchingStrategyRepository : IBaseRepository
    {
        /// <summary>
        /// Creates a new <see cref="MatchingStrategy"/>.
        /// </summary>
        /// <param name="strategy">The strategy to create.</param>
        /// <returns>The created strategy's id.</returns>
        Task<Guid> CreateAsync(MatchingStrategy strategy);

        /// <summary>
        /// Updates an existing strategy.
        /// </summary>
        /// <param name="strategy">The strategy to update.</param>
        Task UpdateAsync(MatchingStrategy strategy);

        /// <summary>
        /// Deletes an existing strategy.
        /// </summary>
        /// <param name="strategy">The strategy to delete.</param>
        Task DeleteAsync(MatchingStrategy strategy);

        /// <summary>
        /// Returns the requested strategy or null.
        /// </summary>
        /// <param name="id">The strategy id.</param>
        /// <returns>The requested strategy or null.</returns>
        Task<MatchingStrategy> GetByIdAsync(Guid id);

        /// <summary>
        /// Returns all the strategies.
        /// </summary>
        /// <returns>The list of strategies.</returns>
        Task<List<MatchingStrategy>> GetAllAsync();
    }
}
