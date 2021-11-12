using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Seed
{
    /// <summary>
    /// Inserts the demo users.
    /// </summary>
    public class DemoUsersSeed : IDataSeedContributor
    {
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
            var andrewCrawId = Guid.Parse("26a02c40-c303-447a-ab58-c0a98c1341ea");
            var andrewCraw2Id = Guid.Parse("a35ad06e-33fb-4201-bd38-e3da29445676");
            var andrewCrawWithIdentificationNumberId = Guid.Parse("aada3c0c-bbaf-490c-9078-8b8503a5982d");
            var aCrawId = Guid.Parse("3215bd09-a18b-4f70-b1f8-24ad33c7bc1d");
            var pettySmithId = Guid.Parse("7c922928-fe5f-417f-9017-d882b23be5ce");
            var pettySmithWithIdentificationNumberId = Guid.Parse("3677d252-7dc6-4562-b9c4-ab1e19648530");

            var people = await _personRepository.GetAllAsync();

            if (people.All(p => p.Id != andrewCrawId))
            {
                var person = Person.Factory.Create(
                    andrewCrawId,
                    "Andrew",
                    "Craw",
                    DateTime.Parse("1985-02-20"),
                    identificationNumber: null);

                await _personRepository.CreateAsync(person);

                _logger.LogInformation("Created {FirstName} {LastName} ({BirthDate}) Id = {IdentificationNumber}", person.FirstName, person.LastName, person.BirthDate, person.IdentificationNumber);
            }

            if (people.All(p => p.Id != andrewCraw2Id))
            {
                var person = Person.Factory.Create(
                    andrewCraw2Id,
                    "Andrew",
                    "Craw",
                    birthDate: null,
                    identificationNumber: null);

                await _personRepository.CreateAsync(person);

                _logger.LogInformation("Created {FirstName} {LastName} ({BirthDate}) Id = {IdentificationNumber}", person.FirstName, person.LastName, person.BirthDate, person.IdentificationNumber);
            }

            if (people.All(p => p.Id != pettySmithId))
            {
                var person = Person.Factory.Create(
                    pettySmithId,
                    "Petty",
                    "Smith",
                    DateTime.Parse("1985-02-20"),
                    identificationNumber: null);

                await _personRepository.CreateAsync(person);

                _logger.LogInformation("Created {FirstName} {LastName} ({BirthDate}) Id = {IdentificationNumber}", person.FirstName, person.LastName, person.BirthDate, person.IdentificationNumber);
            }

            if (people.All(p => p.Id != aCrawId))
            {
                var person = Person.Factory.Create(
                    aCrawId,
                    "A.",
                    "Craw",
                    DateTime.Parse("1985-02-20"),
                    identificationNumber: null);

                await _personRepository.CreateAsync(person);

                _logger.LogInformation("Created {FirstName} {LastName} ({BirthDate}) Id = {IdentificationNumber}", person.FirstName, person.LastName, person.BirthDate, person.IdentificationNumber);
            }

            if (people.All(p => p.Id != andrewCrawWithIdentificationNumberId))
            {
                var person = Person.Factory.Create(
                    andrewCrawWithIdentificationNumberId,
                    "Andrew",
                    "Craw",
                    DateTime.Parse("1985-02-20"),
                    identificationNumber: "931212312");

                await _personRepository.CreateAsync(person);

                _logger.LogInformation("Created {FirstName} {LastName} ({BirthDate}) Id = {IdentificationNumber}", person.FirstName, person.LastName, person.BirthDate, person.IdentificationNumber);
            }

            if (people.All(p => p.Id != pettySmithWithIdentificationNumberId))
            {
                var person = Person.Factory.Create(
                    pettySmithWithIdentificationNumberId,
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
