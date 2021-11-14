using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Cache;
using Application.Contracts;
using Application.Contracts.Rules;
using Application.Exceptions;
using Application.Extensions;
using Domain.Exceptions;
using Domain.Model;
using Domain.Repositories;
using Domain.Extensions;
using Domain.Rules;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Namotion.Reflection;

namespace Application.Services
{
    /// <inheritdoc />
    public class StrategyMatchApplicationService : IStrategyMatchApplicationService
    {
        private readonly IMatchingStrategyRepository _strategyRepository;
        private readonly ICustomMemoryCache _memoryCache;
        private readonly ILogger<StrategyMatchApplicationService> _logger;

        /// <summary>
        /// Creates the application service.
        /// </summary>
        public StrategyMatchApplicationService(
            IMatchingStrategyRepository strategyRepository,
            ICustomMemoryCache memoryCache,
            ILogger<StrategyMatchApplicationService> logger)
        {
            _strategyRepository = strategyRepository;
            _memoryCache = memoryCache;
            _logger = logger;

            // Note: we could also have used MediatoR to split each use case in a separate command.
        }

        /// <inheritdoc />
        public async Task<CreateStrategyReplyDto> CreateStrategy(CreateStrategyDto input)
        {
            try
            {
                var rules = new List<MatchingRule>();

                foreach (var ruleDto in input.Rules.OrEmptyIfNull())
                {
                    var parameters = ruleDto.Parameters
                        .OrEmptyIfNull()
                        .Select(parameterDto => MatchingRuleParameter.Factory.Create(parameterDto.Name, parameterDto.Value))
                        .ToList();

                    var rule = MatchingRule.Factory.Create(
                        ruleDto.RuleTypeAssemblyQualifiedName,
                        ruleDto.Name,
                        ruleDto.Description,
                        ruleDto.IsEnabled,
                        parameters);

                    rules.Add(rule);
                }

                var strategy = MatchingStrategy.Factory.Create(
                    input.Name,
                    input.Description,
                    rules);

                await _strategyRepository.CreateAsync(strategy);
                await _strategyRepository.SaveChangesAsync(); // simulate UnitOfWork

                return new CreateStrategyReplyDto()
                {
                    Id = strategy.Id,
                };
            }
            catch (ValidationException validationException)
            {
                _logger.LogDebug(validationException, "Validation exception");
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<StrategyDto> GetByIdAsync(Guid strategyId)
        {
            var strategy = await _strategyRepository.GetByIdAsync(strategyId);
            return strategy?.ToDto();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<StrategyDto>> GetAllAsync()
        {
            var strategies = await _strategyRepository.GetAllAsync();
            return strategies.Select(s => s.ToDto());
        }

        /// <inheritdoc />
        public async Task<bool> DeleteStrategyAsync(Guid strategyId)
        {
            var strategy = await _strategyRepository.GetByIdAsync(strategyId);

            if (strategy is null)
            {
                return false;
            }

            await _strategyRepository.DeleteAsync(strategy);
            await _strategyRepository.SaveChangesAsync(); // simulate UnitOfWork

            // remove entry from cache. Note: this should be isolated in an handler that is listening for domain events (eg. MatchingStrategyDeletedEvent)!
            _memoryCache.RemoveIf(key => ((string) key).Contains(strategy.Id.ToString()));

            return true;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateStrategyAsync(UpdateStrategyDto input)
        {
            try
            {
                var strategy = await _strategyRepository.GetByIdAsync(input.Id);

                var rules = new List<MatchingRule>();

                foreach (var ruleDto in input.Rules.OrEmptyIfNull())
                {
                    var parameters = ruleDto.Parameters
                        .OrEmptyIfNull()
                        .Select(parameterDto =>
                            MatchingRuleParameter.Factory.Create(parameterDto.Name, parameterDto.Value))
                        .ToList();

                    var rule = MatchingRule.Factory.Create(
                        ruleDto.RuleTypeAssemblyQualifiedName,
                        ruleDto.Name,
                        ruleDto.Description,
                        ruleDto.IsEnabled,
                        parameters);

                    rules.Add(rule);
                }

                strategy.Update(
                    input.Name,
                    input.Description,
                    rules);

                await _strategyRepository.UpdateAsync(strategy);
                await _strategyRepository.SaveChangesAsync(); // simulate UnitOfWork

                // remove entry from cache. Note: this should be isolated in an handler that is listening for domain events (eg. MatchingStrategyDeletedEvent)!
                _memoryCache.RemoveIf(key => ((string) key).Contains(strategy.Id.ToString()));


                return true;
            }
            catch (ValidationException validationException)
            {
                _logger.LogDebug(validationException, "Validation exception");
                return false;
            }
        }

        /// <inheritdoc />
        public Task<IEnumerable<RuleDto>> GetAvailableRulesAsync()
        {
            if (_memoryCache.TryGetValue(CacheKeys.AvailableRules, out var cachedRules))
            {
                return Task.FromResult((IEnumerable<RuleDto>)cachedRules);
            }

            var ruleContributorType = typeof(IMatchingRuleContributor);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .AsParallel()
                .SelectMany(x => x.GetTypes())
                .Where(x => !x.IsInterface && !x.IsAbstract)
                .Where(x => ruleContributorType.IsAssignableFrom(x));

            var rules = new List<RuleDto>();

            foreach (var type in types)
            {
                var ruleParameterAttributes = type
                    .GetCustomAttributes(typeof(RuleParameterAttribute), inherit: true)
                    .Select(t => (RuleParameterAttribute)t);

                var ruleTypeDto = new RuleDto()
                {
                    AssemblyQualifiedName = type.GetAssemblyQualifiedName(),
                    Description = type.GetXmlDocsSummary(),
                    Parameters = ruleParameterAttributes.Select(a => new RuleDto.ParameterDto()
                    {
                        Name = a.ParameterName,
                        Description = a.ParameterDescription,
                    }).ToList(),
                };

                rules.Add(ruleTypeDto);
            }

            _memoryCache.Set(CacheKeys.AvailableRules, rules); // does not have a expire date as it saved in memory and it can change only after app restart

            return Task.FromResult((IEnumerable<RuleDto>)rules);
        }
    }
}
