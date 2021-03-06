using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Extensions;
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
        public const string AndrewCrawId = "26a02c40-c303-447a-ab58-c0a98c1341ea";
        public const string AndrewCraw2Id = "a35ad06e-33fb-4201-bd38-e3da29445676";
        public const string AndrewCrawWithIdentificationNumberId = "aada3c0c-bbaf-490c-9078-8b8503a5982d";
        public const string ACrawId = "3215bd09-a18b-4f70-b1f8-24ad33c7bc1d";
        public const string PettySmithId = "7c922928-fe5f-417f-9017-d882b23be5ce";
        public const string PettySmithWithIdentificationNumberId = "3677d252-7dc6-4562-b9c4-ab1e19648530";
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
            // Note: we should check only interested ids and not load the entire table in memory!
            var people = await _personRepository.GetAllAsync();

            var peopleToCreate = new List<Person>()
            {
                Person.Factory.Create(
                    AndrewCrawId.ToGuid(),
                    "Andrew",
                    "Craw",
                    DateTime.Parse("1985-02-20"),
                    identificationNumber: null),
                Person.Factory.Create(
                    AndrewCraw2Id.ToGuid(),
                    "Andrew",
                    "Craw",
                    birthDate: null,
                    identificationNumber: null),
                Person.Factory.Create(
                    PettySmithId.ToGuid(),
                    "Petty",
                    "Smith",
                    DateTime.Parse("1985-02-20"),
                    identificationNumber: null),
                Person.Factory.Create(
                    ACrawId.ToGuid(),
                    "A.",
                    "Craw",
                    DateTime.Parse("1985-02-20"),
                    identificationNumber: null),
                Person.Factory.Create(
                    AndrewCrawWithIdentificationNumberId.ToGuid(),
                    "Andrew",
                    "Craw",
                    DateTime.Parse("1985-02-20"),
                    identificationNumber: "931212312"),
                Person.Factory.Create(
                    PettySmithWithIdentificationNumberId.ToGuid(),
                    "Petty",
                    "Smith",
                    DateTime.Parse("1985-02-20"),
                    identificationNumber: "931212312"),
            };

            var createdEntries = 0;
            foreach (var person in peopleToCreate)
            {
                if (people.All(p => p.Id != person.Id))
                {
                    await _personRepository.CreateAsync(person);
                    createdEntries++;
                    _logger.LogInformation("Seeded person {Guid}", person.Id);
                }
            }

            await _personRepository.SaveChangesAsync(); // simulate UnitOfWork

            if (createdEntries == 0)
            {
                _logger.LogInformation("Default people found. Skipping seed");
            }
            else
            {
                _logger.LogInformation("People seed completed. Created {Number} people", createdEntries);
            }
        }
    }
}
