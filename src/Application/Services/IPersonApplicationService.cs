using System;
using System.Threading.Tasks;
using Application.Contracts.Person;

namespace Application.Services
{
    /// <summary>
    /// Contains all the use cases that involve the <see cref="Person"/> entity.
    /// </summary>
    public interface IPersonApplicationService
    {
        /// <summary>
        /// Creates a new <see cref="Person"/> and returns its id. If the operation fails an exception is thrown.
        /// </summary>
        /// <param name="input">The person to be created.</param>
        /// <returns>The created person's id.</returns>
        Task<Guid> CreatePersonAsync(CreatePersonDto input);

        Task<decimal> CalculateProbabilitySameIdentity(Guid firstPersonId, Guid secondPersonId);
    }
}
