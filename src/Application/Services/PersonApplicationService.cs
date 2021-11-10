using System;
using System.Threading.Tasks;
using Application.Contracts.Person;
using Application.Rules;
using Domain.Model;
using Domain.Repositories;

namespace Application.Services
{
    /// <inheritdoc />
    public class PersonApplicationService : IPersonApplicationService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IPersonMatchingStrategyRepository _personMatchingStrategyRepository;
        private readonly IPersonMatchingRuleStrategyExecutor _matchingRuleStrategyExecutor;

        public PersonApplicationService(
            IPersonRepository personRepository,
            IPersonMatchingStrategyRepository personMatchingStrategyRepository,
            IPersonMatchingRuleStrategyExecutor matchingRuleStrategyExecutor)
        {
            _personRepository = personRepository;
            _personMatchingStrategyRepository = personMatchingStrategyRepository;
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
            var strategy = await _personMatchingStrategyRepository.GetByNameAsync("Default");

            var firstPerson = await _personRepository.GetByIdAsync(firstPersonId);
            var secondPerson = await _personRepository.GetByIdAsync(secondPersonId);

            var score = await _matchingRuleStrategyExecutor.ExecuteAsync(strategy, firstPerson, secondPerson);

            return score;
        }
    }
}
