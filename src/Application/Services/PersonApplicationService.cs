using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Person;
using Application.Exceptions;
using Application.Rules;
using Domain.Model;
using Domain.Repositories;
using Domain.Rules;

namespace Application.Services
{
    /// <inheritdoc />
    public class PersonApplicationService : IPersonApplicationService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IMatchingStrategyRepository _matchingStrategyRepository;
        private readonly IMatchingRuleStrategyExecutor _matchingRuleStrategyExecutor;

        /// <summary>
        /// Creates the application service.
        /// </summary>
        public PersonApplicationService(
            IPersonRepository personRepository,
            IMatchingStrategyRepository matchingStrategyRepository,
            IMatchingRuleStrategyExecutor matchingRuleStrategyExecutor)
        {
            _personRepository = personRepository;
            _matchingStrategyRepository = matchingStrategyRepository;
            _matchingRuleStrategyExecutor = matchingRuleStrategyExecutor;
        }

        /// <inheritdoc />
        public async Task<CreatePersonReplyDto> CreatePersonAsync(CreatePersonDto input)
        {
            var person = Person.Factory.Create(
                input.FirstName,
                input.LastName,
                input.BirthDate,
                input.IdentificationNumber);

            await _personRepository.CreateAsync(person);

            return new CreatePersonReplyDto()
            {
                Id = person.Id,
            };
        }

        /// <inheritdoc />
        public async Task<ProbabilitySameIdentityDto> CalculateProbabilitySameIdentity(Guid firstPersonId, Guid secondPersonId, string strategyName)
        {
            var strategy = await _matchingStrategyRepository.GetByNameAsync(strategyName ?? "Default");

            if (strategy is null)
            {
                throw new StrategyNotFoundException(strategyName);
            }

            var firstPerson = await _personRepository.GetByIdAsync(firstPersonId);

            if (firstPerson is null)
            {
                throw new PersonNotFoundException(firstPersonId);
            }

            var secondPerson = await _personRepository.GetByIdAsync(secondPersonId);

            if (secondPerson is null)
            {
                throw new PersonNotFoundException(secondPersonId);
            }

            var probabilitySameIdentity = await _matchingRuleStrategyExecutor.ExecuteAsync(strategy, firstPerson, secondPerson);

            return new ProbabilitySameIdentityDto()
            {
                Probability = probabilitySameIdentity.Probability,
                Contributors = probabilitySameIdentity.Contributors.Select(c => new ProbabilitySameIdentityDto.ContributorDto()
                {
                    Name = c.Name,
                    Description = c.Description,
                    RuleType = c.RuleType,
                    Value = c.Value,
                }).ToList()
            };
        }

        /// <inheritdoc />
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
