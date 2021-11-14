using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Cache;
using Application.Contracts.Person;
using Domain.Exceptions;
using Domain.Model;
using Domain.Repositories;
using Domain.Rules;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    /// <inheritdoc />
    public class PersonApplicationService : IPersonApplicationService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IMatchingStrategyRepository _matchingStrategyRepository;
        private readonly IMatchingRuleStrategyExecutor _matchingRuleStrategyExecutor;
        private readonly ICustomMemoryCache _memoryCache;
        private readonly ILogger<PersonApplicationService> _logger;

        /// <summary>
        /// Creates the application service.
        /// </summary>
        public PersonApplicationService(
            IPersonRepository personRepository,
            IMatchingStrategyRepository matchingStrategyRepository,
            IMatchingRuleStrategyExecutor matchingRuleStrategyExecutor,
            ICustomMemoryCache memoryCache,
            ILogger<PersonApplicationService> logger)
        {
            _personRepository = personRepository;
            _matchingStrategyRepository = matchingStrategyRepository;
            _matchingRuleStrategyExecutor = matchingRuleStrategyExecutor;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<CreatePersonReplyDto> CreatePersonAsync(CreatePersonDto input)
        {
            try
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
            catch (ValidationException validationException)
            {
                _logger.LogDebug(validationException, "Validation errors");
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<ProbabilitySameIdentityDto> CalculateProbabilitySameIdentity(CalculateProbabilitySameIdentityRequestDto input)
        {
            var cacheKey = CacheKeys.CalculateProbabilitySameIdentity(input.FirstPersonId, input.SecondPersonId, input.StrategyId);
            if (_memoryCache.TryGetValue(cacheKey, out ProbabilitySameIdentityDto cacheEntry))
            {
                return cacheEntry;
            }

            var strategy = await _matchingStrategyRepository.GetByIdAsync(input.StrategyId);

            if (strategy is null)
            {
                return null;
            }

            var firstPerson = await _personRepository.GetByIdAsync(input.FirstPersonId);

            if (firstPerson is null)
            {
                return null;
            }

            var secondPerson = await _personRepository.GetByIdAsync(input.SecondPersonId);

            if (secondPerson is null)
            {
                return null;
            }

            var probabilitySameIdentity = await _matchingRuleStrategyExecutor.ExecuteAsync(strategy, firstPerson, secondPerson);

            var result = new ProbabilitySameIdentityDto()
            {
                Probability = probabilitySameIdentity.Probability,
                Contributors = probabilitySameIdentity.Contributors.Select(c => new ProbabilitySameIdentityDto.ContributorDto()
                {
                    Name = c.Name,
                    Description = c.Description,
                    RuleType = c.RuleType,
                    Value = c.Value,
                }).ToList(),
                Strategy = new ProbabilitySameIdentityDto.StrategyDto()
                {
                    Id = strategy.Id,
                    Name = strategy.Name,
                    Description = strategy.Description,
                },
            };

            // insert in cache for 10min. Must be evicted if MatchingStrategy or Person change
            _memoryCache.Set(cacheKey, result, absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(10));

            return result;
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
