using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Person;
using Application.Exceptions;
using Application.Rules;
using Domain.Model;
using Domain.Repositories;

namespace Application.Services
{
    /// <inheritdoc />
    public class PersonApplicationService : IPersonApplicationService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IMatchingStrategyRepository _matchingStrategyRepository;
        private readonly IMatchingRuleStrategyExecutor _matchingRuleStrategyExecutor;

        public PersonApplicationService(
            IPersonRepository personRepository,
            IMatchingStrategyRepository matchingStrategyRepository,
            IMatchingRuleStrategyExecutor matchingRuleStrategyExecutor)
        {
            _personRepository = personRepository;
            _matchingStrategyRepository = matchingStrategyRepository;
            _matchingRuleStrategyExecutor = matchingRuleStrategyExecutor;
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
            var strategy = await _matchingStrategyRepository.GetByNameAsync("Default");

            var firstPerson = await _personRepository.GetByIdAsync(firstPersonId);

            if (firstPerson is null)
            {
                throw new UserNotFoundException(firstPersonId);
            }

            var secondPerson = await _personRepository.GetByIdAsync(secondPersonId);

            if (secondPerson is null)
            {
                throw new UserNotFoundException(secondPersonId);
            }

            var score = await _matchingRuleStrategyExecutor.ExecuteAsync(strategy, firstPerson, secondPerson);

            return score;
        }

        public async Task<IEnumerable<PersonDto>> GetPeople()
        {
            var people = await _personRepository.GetAllAsync();
            return people.Select(p => new PersonDto()
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                BirthDate = p.BirthDate,
                IdentificationNumber = p.IdentificationNumber,
            });
        }
    }
}
