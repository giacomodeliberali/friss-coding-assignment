using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Rules;
using Application.Services;
using Domain.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ValidationException = Domain.Exceptions.ValidationException;

namespace Web
{
    /// <summary>
    /// Manages the CRUD operations on the MatchingStrategy aggregate.
    /// </summary>
    [Route("api/strategies")]
    public class StrategyController : CustomBaseController
    {
        private readonly IStrategyMatchApplicationService _strategyMatchApplicationService;
        private readonly ILogger<StrategyController> _logger;

        /// <inheritdoc />
        public StrategyController(
            IHostingEnvironment hostingEnvironment,
            IStrategyMatchApplicationService strategyMatchApplicationService,
            ILogger<StrategyController> logger)
            : base(hostingEnvironment)
        {
            _strategyMatchApplicationService = strategyMatchApplicationService;
            _logger = logger;
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
            try
            {
                var result = await _strategyMatchApplicationService.CreateStrategy(input);
                return Created(string.Empty, result);
            }
            catch (ValidationException validationException)
            {
                _logger.LogDebug(validationException, "Invalid request parameters");
                return BadRequest(validationException);
            }
            catch (BusinessException businessException)
            {
                _logger.LogWarning(businessException, "Error during strategy creation");
                return ServerError(businessException);
            }
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
                return BadRequest(new ValidationException("Path id and body id do not match."));
            }

            try
            {
                await _strategyMatchApplicationService.UpdateStrategyAsync(input);
                return Ok();
            }
            catch (ValidationException validationException)
            {
                _logger.LogDebug(validationException, "Invalid request parameters");
                return BadRequest(validationException);
            }
            catch (BusinessException businessException)
            {
                _logger.LogWarning(businessException, "Error during strategy update");
                return ServerError(businessException);
            }
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
                _logger.LogDebug("Strategy with id '{Id}' not found", id);
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
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteByIdAsync(Guid id)
        {
            try
            {
                await _strategyMatchApplicationService.DeleteStrategyAsync(id);
                return NoContent();
            }
            catch (BusinessException businessException)
            {
                _logger.LogWarning(businessException, "Error deleting strategy with id '{Id}'", id);
                return ServerError(businessException);
            }
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
