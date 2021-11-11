using System;
using System.Threading.Tasks;
using Domain.Model;

namespace Domain.Repositories
{
    /// <summary>
    /// The repository for the <see cref="MatchingStrategy"/> aggregate root.
    /// </summary>
    public interface IMatchingStrategyRepository
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
        /// Returns the requested strategy by name or null.
        /// </summary>
        /// <param name="name">The strategy name.</param>
        /// <returns>The requested strategy or null.</returns>
        Task<MatchingStrategy> GetByNameAsync(string name);
    }
}
