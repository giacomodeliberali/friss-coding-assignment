using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Contracts.Person;
using Domain.Model;
using Domain.Repositories;
using Domain.Rules;

namespace Application.Services
{
    /// <inheritdoc />
    public class PersonApplicationService : IPersonApplicationService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IPersonMatchingStrategyRepository _personMatchingStrategyRepository;

        public PersonApplicationService(
            IPersonRepository personRepository,
            IPersonMatchingStrategyRepository personMatchingStrategyRepository)
        {
            _personRepository = personRepository;
            _personMatchingStrategyRepository = personMatchingStrategyRepository;
        }

        public async Task<Guid> CreatePersonAsync(CreatePersonDto input)
        {
            var person = Person.Factory.Create(
                input.FirstName,
                input.LastName,
                input.BirthDate,
                input.IdentificationNumber);

            await _personRepository.CreateAsync(person);

            return person.Id;
        }

        public async Task<decimal> CalculateProbabilitySameIdentity(Guid firstPersonId, Guid secondPersonId)
        {
            var strategy = await _personMatchingStrategyRepository.GetByNameAsync("Default");

            var firstPerson = await _personRepository.GetByIdAsync(firstPersonId);
            var secondPerson = await _personRepository.GetByIdAsync(secondPersonId);

            return await strategy.Match(firstPerson, secondPerson);
        }
    }
}
