using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Model;

namespace Domain.Repositories
{
    /// <summary>
    /// The repository for the <see cref="Person"/>.
    /// </summary>
    public interface IPersonRepository
    {
        /// <summary>
        /// Creates a new <see cref="Person"/>. If the operation fails an exception in thrown.
        /// </summary>
        /// <param name="person">The <see cref="Person"/> to be created</param>
        Task CreateAsync(Person person);

        /// <summary>
        /// Returns the requested <see cref="Person"/> or null if it can not be found.
        /// </summary>
        /// <param name="id">The id of the <see cref="Person"/> to retrieve</param>
        /// <returns>The <see cref="Person"/> or null</returns>
        Task<Person> GetByIdAsync(Guid id);

        /// <summary>
        /// Get all the registered people.
        /// Note: this should be moved in a QueryService.
        /// </summary>
        /// <returns>The list of people.</returns>
        Task<List<Person>> GetAllAsync();
    }
}
