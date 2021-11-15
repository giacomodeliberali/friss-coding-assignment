using System;
using Domain.Exceptions;
using Domain.Extensions;
using WriteModel;

namespace Domain.Model
{
    /// <summary>
    /// A person that is registered into the system.
    /// </summary>
    public class Person : Entity
    {
        /// <summary>
        /// The first name.
        /// </summary>
        public string FirstName { get; private set; }

        /// <summary>
        /// The last name.
        /// </summary>
        public string LastName { get; private set; }

        /// <summary>
        /// The birth date.
        /// </summary>
        public DateTime? BirthDate { get; private set; }

        /// <summary>
        /// The business identifier.
        /// </summary>
        public string IdentificationNumber { get; private set; }

        private PersonData _snapshot;

        /// <summary>
        /// The write model snapshot.
        /// </summary>
        public PersonData Snapshot => _snapshot;

        private Person()
        {
        }

        /// <summary>
        /// Factory used to create validated instances of <see cref="Person"/>.
        /// </summary>
        public static class Factory
        {
            /// <summary>
            /// Creates a new validated instances of <see cref="Person"/>.
            /// </summary>
            /// <param name="firstName">The first name.</param>
            /// <param name="lastName">The last name.</param>
            /// <param name="birthDate">The birth date.</param>
            /// <param name="identificationNumber">The business identification number.</param>
            /// <exception cref="ValidationException">When parameters are invalid.</exception>
            /// <returns>A new validated instances of <see cref="Person"/>.</returns>
            public static Person Create(
                string firstName,
                string lastName,
                DateTime? birthDate,
                string identificationNumber)
            {
                return Create(
                    Guid.NewGuid(),
                    firstName,
                    lastName,
                    birthDate,
                    identificationNumber);
            }

            /// <summary>
            /// Creates a new validated instances of <see cref="Person"/> using the specified id.
            /// </summary>
            /// <param name="id">The id to use.</param>
            /// <param name="firstName">The first name.</param>
            /// <param name="lastName">The last name.</param>
            /// <param name="birthDate">The birth date (time is not considered).</param>
            /// <param name="identificationNumber">The business identification number.</param>
            /// <exception cref="ValidationException">When parameters are invalid.</exception>
            /// <returns>A new validated instances of <see cref="Person"/>.</returns>
            /// <returns></returns>
            public static Person Create(
                Guid id,
                string firstName,
                string lastName,
                DateTime? birthDate,
                string identificationNumber)
            {
                firstName.ThrowIfNullOrEmpty(nameof(firstName));
                lastName.ThrowIfNullOrEmpty(nameof(lastName));

                return new Person()
                {
                    Id = id,
                    FirstName = firstName.Trim(),
                    LastName = lastName.Trim(),
                    BirthDate = birthDate?.Date, // do not save time. Note: the timezone is not handled, we should only accept UTC
                    IdentificationNumber = identificationNumber?.Trim(),
                };
            }

            /// <summary>
            /// Recreates the <see cref="Person"/> from the database.
            /// </summary>
            /// <param name="snapshot">The write model snapshot.</param>
            /// <returns>A new validated instances of <see cref="Person"/>.</returns>
            public static Person FromSnapshot(PersonData snapshot)
            {
                return new Person()
                {
                    Id = snapshot.Id,
                    FirstName = snapshot.FirstName,
                    LastName = snapshot.LastName,
                    BirthDate = snapshot.BirthDate,
                    IdentificationNumber = snapshot.IdentificationNumber,
                    _snapshot = snapshot,
                };
            }
        }
    }
}
