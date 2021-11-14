using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Person;
using Application.Services;
using Domain.Exceptions;
using Domain.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ValidationException = Domain.Exceptions.ValidationException;

namespace Web
{
    /// <summary>
    /// Manages the CRUD operations on the Person entity and exposes
    /// the method to calculate the probability that two people are the same.
    /// <remarks>We should deal here with authentication and authorization.</remarks>
    /// </summary>
    [ApiController]
    [Route("api/people")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonApplicationService _personApplicationService;

        /// <inheritdoc />
        public PersonController(IPersonApplicationService personApplicationService)
        {
            _personApplicationService = personApplicationService;
        }

        /// <summary>
        /// Creates and persists a new Person.
        /// </summary>
        /// <param name="input">The person to create.</param>
        /// <returns>The created person's id.</returns>
        [ProducesResponseType(typeof(CreatePersonReplyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([Required] CreatePersonDto input)
        {
            var result = await _personApplicationService.CreatePersonAsync(input);

            if (result is null)
            {
                return BadRequest();
            }

            return Created(string.Empty, result);
        }

        /// <summary>
        /// Returns the list of all created people.
        /// </summary>
        /// <returns>The list of people.</returns>
        [ProducesResponseType(typeof(IEnumerable<PersonDto>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var people = await _personApplicationService.GetPeople();
            return Ok(people);
        }

        /// <summary>
        /// Calculates the probability that the two given people are the same identity.
        /// It uses the provided strategy or the "Default" one if none is specified.
        /// </summary>
        /// <param name="input">The dto with the <see cref="Person"/> to compare and the strategy to use.</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ProbabilitySameIdentityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [HttpGet("probability-same-identity")]
        public async Task<IActionResult> CalculateProbabilitySameIdentity([Required] [FromQuery] CalculateProbabilitySameIdentityRequestDto input)
        {
            var probabilitySameIdentity = await _personApplicationService.CalculateProbabilitySameIdentity(input);

            if (probabilitySameIdentity is null)
            {
                return BadRequest();
            }

            return Ok(probabilitySameIdentity);
        }
    }
}
