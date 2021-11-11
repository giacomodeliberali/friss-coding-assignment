using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Rules;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ValidationException = Domain.Exceptions.ValidationException;

namespace Web
{
    /// <summary>
    /// Manages the CRUD operations on the MatchingStrategy aggregate.
    /// </summary>
    [ApiController]
    [Route("api/strategies")]
    public class StrategyController : ControllerBase
    {
        private readonly IStrategyMatchApplicationService _strategyMatchApplicationService;

        /// <inheritdoc />
        public StrategyController(IStrategyMatchApplicationService strategyMatchApplicationService)
        {
            _strategyMatchApplicationService = strategyMatchApplicationService;
        }

        /// <summary>
        /// Creates a new strategy. The strategy name must be unique.
        /// </summary>
        /// <param name="input">The strategy to create.</param>
        /// <returns>The created strategy's id.</returns>
        [ProducesResponseType(typeof(CreateStrategyReplyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([Required] CreateStrategyDto input)
        {
            var result = await _strategyMatchApplicationService.CreateStrategy(input);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// Updates an existing strategy.
        /// </summary>
        /// <param name="id">The strategy id.</param>
        /// <param name="input">The strategy to update.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [Required] UpdateStrategyDto input)
        {
            if (id != input.Id)
            {
                throw new ValidationException("Path id and body id do not match.");
            }

            await _strategyMatchApplicationService.UpdateStrategyAsync(input);

            return Ok();
        }

        /// <summary>
        /// Returns the requested strategy.
        /// </summary>
        /// <param name="id">The strategy id.</param>
        /// <returns>The requested strategy.</returns>
        [ProducesResponseType(typeof(StrategyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _strategyMatchApplicationService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Deletes the requested strategy.
        /// </summary>
        /// <param name="id">The strategy to delete.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteByIdAsync(Guid id)
        {
            await _strategyMatchApplicationService.DeleteStrategyAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Returns the list of available rules to compose a strategy.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(List<RuleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        [HttpGet("available-rules")]
        public async Task<IActionResult> GetAvailableRulesAsync()
        {
            var result = await _strategyMatchApplicationService.GetAvailableRulesAsync();
            return Ok(result);
        }
    }
}
