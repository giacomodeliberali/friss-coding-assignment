using System;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Rules;

namespace Application.Services
{
    public interface IStrategyMatchApplicationService
    {
        /// <summary>
        /// Creates a new strategy with the given rules and parameters. Throws if the operation fails.
        /// </summary>
        /// <param name="input">The strategy to create.</param>
        /// <returns>The created strategy's id.</returns>
        Task<Guid> CreateStrategy(CreateStrategyDto input);

        Task<StrategyDto> GetByIdAsync(Guid strategyId);

        Task DeleteStrategy(Guid id);

        Task UpdateStrategyAsync(UpdateStrategyDto input);
    }
}
