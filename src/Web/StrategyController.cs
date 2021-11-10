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
        private readonly IPersonStrategyMatchApplicationService _personStrategyMatchApplicationService;

        public StrategyController(IPersonStrategyMatchApplicationService personStrategyMatchApplicationService)
        {
            _personStrategyMatchApplicationService = personStrategyMatchApplicationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([Required] CreateStrategyDto input)
        {
            var guid = await _personStrategyMatchApplicationService.CreateStrategy(input);
            return Ok(guid);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var strategyDto = await _personStrategyMatchApplicationService.GetByIdAsync(id);

            if (strategyDto == null)
            {
                return NotFound();
            }

            return Ok(strategyDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(Guid id)
        {
            await _personStrategyMatchApplicationService.DeleteStrategy(id);
            return Ok();
        }
    }
}
