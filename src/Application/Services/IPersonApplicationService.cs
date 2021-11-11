using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Contracts.Person;
using Domain.Exceptions;
using Domain.Model;

namespace Application.Services
{
    /// <summary>
    /// Contains all the use cases that involve the <see cref="Person"/> entity.
    /// </summary>
    public interface IPersonApplicationService
    {
        /// <summary>
        /// Creates a new <see cref="Person"/> and returns its id.
        /// </summary>
        /// <param name="input">The person to be created.</param>
        /// <returns>The created person's id.</returns>
        /// <exception cref="ValidationException">When input parameters are invalid.</exception>
        Task<CreatePersonReplyDto> CreatePersonAsync(CreatePersonDto input);

        /// <summary>
        /// Calculates the probability that the two given people are the same identity using the provided strategy.
        /// </summary>
        /// <param name="firstPersonId">The first <see cref="Person"/> to compare.</param>
        /// <param name="secondPersonId">The second <see cref="Person"/> to compare.</param>
        /// <param name="strategyName">The name of the strategy to use.</param>
        /// <returns>The probability (0-1) the the two people's identities match.</returns>
        Task<decimal> CalculateProbabilitySameIdentity(Guid firstPersonId, Guid secondPersonId, string strategyName);

        /// <summary>
        /// Returns all the created people.
        /// </summary>
        /// <returns>The list of <see cref="Person"/></returns>
        Task<IEnumerable<PersonDto>> GetPeople();
    }
}
