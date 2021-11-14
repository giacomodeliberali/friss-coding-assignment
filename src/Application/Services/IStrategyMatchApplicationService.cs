using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Rules;
using Domain.Model;
using Domain.Rules;

namespace Application.Services
{
    /// <summary>
    /// Contains all the use cases that involve the <see cref="MatchingStrategy"/>, <see cref="MatchingRule"/> and <see cref="MatchingRuleParameter"/>.
    /// </summary>
    public interface IStrategyMatchApplicationService
    {
        /// <summary>
        /// Creates a new strategy with the given rules and parameters.
        /// </summary>
        /// <param name="input">The strategy to create.</param>
        /// <returns>The created strategy's id.</returns>
        Task<CreateStrategyReplyDto> CreateStrategy(CreateStrategyDto input);

        /// <summary>
        /// Returns the given strategy or null if not found.
        /// </summary>
        /// <param name="strategyId">The strategy id.</param>
        /// <returns>The strategy or null.</returns>
        Task<StrategyDto> GetByIdAsync(Guid strategyId);

        /// <summary>
        /// Returns the list of available strategies.
        /// </summary>
        /// <returns>The list of available strategies.</returns>
        Task<IEnumerable<StrategyDto>> GetAllAsync();

        /// <summary>
        /// Deletes an existing strategy.
        /// </summary>
        /// <param name="strategyId">The strategy to delete.</param>
        Task<bool> DeleteStrategyAsync(Guid strategyId);

        /// <summary>
        /// Updates an existing strategy.
        /// </summary>
        /// <param name="input">The strategy to update.</param>
        Task<bool> UpdateStrategyAsync(UpdateStrategyDto input);

        /// <summary>
        /// Returns the list of available <see cref="IRuleContributor"/> to compose a <see cref="MatchingStrategy"/>.
        /// </summary>
        /// <returns>The list of available rule types.</returns>
        Task<IEnumerable<RuleDto>> GetAvailableRulesAsync();
    }
}
