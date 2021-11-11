using System;
using Domain.Exceptions;
using Domain.Model;

namespace Application.Exceptions
{
    /// <summary>
    /// Thrown when attempting to run a workflow that requires a non-existing <see cref="Person"/>.
    /// </summary>
    public class PersonNotFoundException : BusinessException
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="id">The person id.</param>
        public PersonNotFoundException(Guid id)
        : base($"Person {id} not found.")
        {
        }
    }
}
