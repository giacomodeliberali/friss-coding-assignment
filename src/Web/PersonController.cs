using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.Contracts.Person;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web
{
    [ApiController]
    [Route("api/people")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonApplicationService _personApplicationService;

        public PersonController(IPersonApplicationService personApplicationService)
        {
            _personApplicationService = personApplicationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([Required] CreatePersonDto input)
        {
            var guid = await _personApplicationService.CreatePersonAsync(input);
            return Ok(guid);
        }

        [HttpGet("probability-same-identity")]
        public async Task<IActionResult> CalculateProbabilitySameIdentity([Required] Guid firstPersonId, [Required] Guid secondPersonId)
        {
            var score = await _personApplicationService.CalculateProbabilitySameIdentity(firstPersonId, secondPersonId);
            return Ok(score);
        }
    }
}
