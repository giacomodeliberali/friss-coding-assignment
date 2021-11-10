using System;
using Ardalis.GuardClauses;
using JetBrains.Annotations;
using WriteModel;

namespace Domain.Model
{
    public class Person : Entity
    {
        private PersonData _snapshot;

        public PersonData Snapshot => _snapshot;

        [NotNull]
        public string FirstName { get; private set; }

        [NotNull]
        public string LastName { get; private set; }

        public DateTime? BirthDate { get; private set; }

        [CanBeNull]
        public string IdentificationNumber { get; private set; }

        private Person()
        {
        }

        public static class Factory
        {
            public static Person Create(
                string firstName,
                string lastName,
                DateTime? birthDate,
                string identificationNumber)
            {
                Guard.Against.NullOrEmpty(firstName?.Trim(), $"{nameof(firstName)} can not be empty");
                Guard.Against.NullOrEmpty(lastName?.Trim(), $"{nameof(lastName)} can not be empty");

                return new Person()
                {
                    Id = Guid.NewGuid(), // this guid should be generated sequentially to not slow down MSSQL reindexing at each insert
                    FirstName = firstName.Trim(),
                    LastName = lastName.Trim(),
                    BirthDate = birthDate,
                    IdentificationNumber = identificationNumber?.Trim(),
                };
            }

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
