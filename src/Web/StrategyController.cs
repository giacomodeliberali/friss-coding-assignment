using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Rules;
using Application.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([Required] CreateStrategyDto input)
        {
            var result = await _strategyMatchApplicationService.CreateStrategy(input);

            if (result is null)
            {
                return BadRequest();
            }

            return Created(string.Empty, result);

        }

        /// <summary>
        /// Updates an existing strategy.
        /// </summary>
        /// <param name="id">The strategy id.</param>
        /// <param name="input">The strategy to update.</param>
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [Required] UpdateStrategyDto input)
        {
            if (id != input.Id)
            {
                return BadRequest();
            }

            var isSuccessful = await _strategyMatchApplicationService.UpdateStrategyAsync(input);

            if(isSuccessful)
            {
                return Ok();
            }

            return BadRequest();
        }

        /// <summary>
        /// Returns the list of strategies.
        /// </summary>
        /// <returns>The list of strategies.</returns>
        [ProducesResponseType(typeof(List<StrategyDto>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _strategyMatchApplicationService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Returns the requested strategy.
        /// </summary>
        /// <param name="id">The strategy id.</param>
        /// <returns>The requested strategy.</returns>
        [ProducesResponseType(typeof(StrategyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _strategyMatchApplicationService.GetByIdAsync(id);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Deletes the requested strategy.
        /// </summary>
        /// <param name="id">The strategy to delete.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteByIdAsync(Guid id)
        {
            var isSuccessful = await _strategyMatchApplicationService.DeleteStrategyAsync(id);

            if (isSuccessful)
            {
                return NoContent();
            }

            return BadRequest();

        }

        /// <summary>
        /// Returns the list of available rules to compose a strategy.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(List<RuleDto>), StatusCodes.Status200OK)]
        [HttpGet("available-rules")]
        public async Task<IActionResult> GetAvailableRulesAsync()
        {
            var result = await _strategyMatchApplicationService.GetAvailableRulesAsync();
            return Ok(result);
        }
    }
}
