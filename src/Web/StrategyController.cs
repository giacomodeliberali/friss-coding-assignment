using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.Contracts.Rules;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web
{
    [ApiController]
    [Route("api/strategies")]
    public class StrategyController : ControllerBase
    {
        private readonly IStrategyMatchApplicationService _strategyMatchApplicationService;

        public StrategyController(IStrategyMatchApplicationService strategyMatchApplicationService)
        {
            _strategyMatchApplicationService = strategyMatchApplicationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([Required] CreateStrategyDto input)
        {
            var guid = await _strategyMatchApplicationService.CreateStrategy(input);
            return Ok(guid);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var strategyDto = await _strategyMatchApplicationService.GetByIdAsync(id);

            if (strategyDto == null)
            {
                return NotFound();
            }

            return Ok(strategyDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(Guid id)
        {
            await _strategyMatchApplicationService.DeleteStrategy(id);
            return Ok();
        }
    }
}
