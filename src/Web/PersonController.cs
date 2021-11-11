using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Person;
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
    /// Manages the CRUD operations on the Person entity and exposes
    /// the method to calculate the probability that two people are the same.
    /// </summary>
    [Route("api/people")]
    public class PersonController : CustomBaseController
    {
        private readonly IPersonApplicationService _personApplicationService;
        private readonly ILogger<PersonController> _logger;

        /// <inheritdoc />
        public PersonController(
            IHostingEnvironment hostingEnvironment,
            IPersonApplicationService personApplicationService,
            ILogger<PersonController> logger)
            : base(hostingEnvironment)
        {
            _personApplicationService = personApplicationService;
            _logger = logger;
        }

        /// <summary>
        /// Creates and persists a new Person.
        /// </summary>
        /// <param name="input">The person to create.</param>
        /// <returns>The created person's id.</returns>
        [ProducesResponseType(typeof(CreatePersonReplyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([Required] CreatePersonDto input)
        {
            try
            {
                var result = await _personApplicationService.CreatePersonAsync(input);
                return Created(string.Empty, result);
            }
            catch (ValidationException validationException)
            {
                _logger.LogDebug(validationException, "Invalid request parameters");
                return BadRequest(validationException);
            }
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
        /// <param name="firstPersonId">The first person to compare.</param>
        /// <param name="secondPersonId">The second person to compare.</param>
        /// <param name="strategyName">The strategy to use (or null for "Default").</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        [HttpGet("probability-same-identity")]
        public async Task<IActionResult> CalculateProbabilitySameIdentity(
            [Required] Guid firstPersonId,
            [Required] Guid secondPersonId,
            string strategyName = "Default")
        {
            try
            {
                var probabilitySameIdentity = await _personApplicationService.CalculateProbabilitySameIdentity(
                    firstPersonId,
                    secondPersonId,
                    strategyName);

                return Ok(probabilitySameIdentity);
            }
            catch (BusinessException businessException)
            {
                _logger.LogInformation(businessException, "Error during probability calculation pipeline");
                return ServerError(businessException);
            }
        }
    }
}
