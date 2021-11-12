using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Seed
{
    /// <summary>
    /// Inserts the demo users (<seealso cref="IDataSeedContributor"/> for why this approach is wrong).
    /// </summary>
    public class DemoUsersSeed : IDataSeedContributor
    {
#pragma warning disable 1591
        public static readonly Guid AndrewCrawId = Guid.Parse("26a02c40-c303-447a-ab58-c0a98c1341ea");
        public static readonly Guid  AndrewCraw2Id = Guid.Parse("a35ad06e-33fb-4201-bd38-e3da29445676");
        public static readonly Guid  AndrewCrawWithIdentificationNumberId = Guid.Parse("aada3c0c-bbaf-490c-9078-8b8503a5982d");
        public static readonly Guid  ACrawId = Guid.Parse("3215bd09-a18b-4f70-b1f8-24ad33c7bc1d");
        public static readonly Guid  PettySmithId = Guid.Parse("7c922928-fe5f-417f-9017-d882b23be5ce");
        public static readonly Guid  PettySmithWithIdentificationNumberId = Guid.Parse("3677d252-7dc6-4562-b9c4-ab1e19648530");
#pragma warning restore 1591

        private readonly IPersonRepository _personRepository;
        private readonly ILogger<DemoUsersSeed> _logger;

        /// <summary>
        /// Creates the data seed.
        /// </summary>
        public DemoUsersSeed(
            IPersonRepository personRepository,
            ILogger<DemoUsersSeed> logger)
        {
            _personRepository = personRepository;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task SeedAsync()
        {
            var people = await _personRepository.GetAllAsync();

            if (people.All(p => p.Id != AndrewCrawId))
            {
                var person = Person.Factory.Create(
                    AndrewCrawId,
                    "Andrew",
                    "Craw",
                    DateTime.Parse("1985-02-20"),
                    identificationNumber: null);

                await _personRepository.CreateAsync(person);

                _logger.LogInformation("Created {FirstName} {LastName} ({BirthDate}) Id = {IdentificationNumber}", person.FirstName, person.LastName, person.BirthDate, person.IdentificationNumber);
            }

            if (people.All(p => p.Id != AndrewCraw2Id))
            {
                var person = Person.Factory.Create(
                    AndrewCraw2Id,
                    "Andrew",
                    "Craw",
                    birthDate: null,
                    identificationNumber: null);

                await _personRepository.CreateAsync(person);

                _logger.LogInformation("Created {FirstName} {LastName} ({BirthDate}) Id = {IdentificationNumber}", person.FirstName, person.LastName, person.BirthDate, person.IdentificationNumber);
            }

            if (people.All(p => p.Id != PettySmithId))
            {
                var person = Person.Factory.Create(
                    PettySmithId,
                    "Petty",
                    "Smith",
                    DateTime.Parse("1985-02-20"),
                    identificationNumber: null);

                await _personRepository.CreateAsync(person);

                _logger.LogInformation("Created {FirstName} {LastName} ({BirthDate}) Id = {IdentificationNumber}", person.FirstName, person.LastName, person.BirthDate, person.IdentificationNumber);
            }

            if (people.All(p => p.Id != ACrawId))
            {
                var person = Person.Factory.Create(
                    ACrawId,
                    "A.",
                    "Craw",
                    DateTime.Parse("1985-02-20"),
                    identificationNumber: null);

                await _personRepository.CreateAsync(person);

                _logger.LogInformation("Created {FirstName} {LastName} ({BirthDate}) Id = {IdentificationNumber}", person.FirstName, person.LastName, person.BirthDate, person.IdentificationNumber);
            }

            if (people.All(p => p.Id != AndrewCrawWithIdentificationNumberId))
            {
                var person = Person.Factory.Create(
                    AndrewCrawWithIdentificationNumberId,
                    "Andrew",
                    "Craw",
                    DateTime.Parse("1985-02-20"),
                    identificationNumber: "931212312");

                await _personRepository.CreateAsync(person);

                _logger.LogInformation("Created {FirstName} {LastName} ({BirthDate}) Id = {IdentificationNumber}", person.FirstName, person.LastName, person.BirthDate, person.IdentificationNumber);
            }

            if (people.All(p => p.Id != PettySmithWithIdentificationNumberId))
            {
                var person = Person.Factory.Create(
                    PettySmithWithIdentificationNumberId,
                    "Petty",
                    "Smith",
                    DateTime.Parse("1985-02-20"),
                    identificationNumber: "931212312");

                await _personRepository.CreateAsync(person);

                _logger.LogInformation("Created {FirstName} {LastName} ({BirthDate}) Id = {IdentificationNumber}", person.FirstName, person.LastName, person.BirthDate, person.IdentificationNumber);
            }

            _logger.LogInformation("People seed completed");

        }
    }
}
