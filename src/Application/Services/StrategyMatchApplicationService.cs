using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Rules;
using Application.Exceptions;
using Application.Rules;
using Domain.Model;
using Domain.Repositories;
using Domain.Extensions;
using Domain.Rules;
using Namotion.Reflection;

namespace Application.Services
{
    /// <inheritdoc />
    public class StrategyMatchApplicationService : IStrategyMatchApplicationService
    {
        private readonly IMatchingStrategyRepository _strategyRepository;

        /// <summary>
        /// Creates the application service.
        /// </summary>
        public StrategyMatchApplicationService(
            IMatchingStrategyRepository strategyRepository)
        {
            _strategyRepository = strategyRepository;

            // Note: we could also have use MediatoR to split each use case in a separate command.
        }

        /// <inheritdoc />
        public async Task<CreateStrategyReplyDto> CreateStrategy(CreateStrategyDto input)
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

            var existingStrategy = await _strategyRepository.GetByNameAsync(input.Name);

            if (existingStrategy != null)
            {
                throw new StrategyAlreadyExistsException(existingStrategy.Name);
            }

            await _strategyRepository.CreateAsync(strategy);

            return new CreateStrategyReplyDto()
            {
                Id = strategy.Id,
            };
        }

        /// <inheritdoc />
        public async Task<StrategyDto> GetByIdAsync(Guid strategyId)
        {
            var strategy = await _strategyRepository.GetByIdAsync(strategyId);

            if (strategy == null)
            {
                throw new StrategyNotFoundException(strategyId);
            }

            // we could use automapper
            return new StrategyDto()
            {
                Id = strategy.Id,
                Name = strategy.Name,
                Description = strategy.Description,
                Rules = strategy.Rules.Select(r =>
                {
                    return new StrategyDto.RuleDto()
                    {
                        Name = r.Name,
                        Description = r.Description,
                        IsEnabled = r.IsEnabled,
                        RuleTypeAssemblyQualifiedName = r.RuleType.GetAssemblyQualifiedName(),
                        Parameters = r.Parameters.Select(p =>
                        {
                            return new StrategyDto.RuleDto.ParameterDto()
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Value = p.Value,
                            };
                        }),
                    };
                }),
            };
        }

        /// <inheritdoc />
        public async Task DeleteStrategyAsync(Guid strategyId)
        {
            var strategy = await _strategyRepository.GetByIdAsync(strategyId);

            if (strategy == null)
            {
                throw new StrategyNotFoundException(strategyId);
            }

            await _strategyRepository.DeleteAsync(strategy);
        }

        /// <inheritdoc />
        public async Task UpdateStrategyAsync(UpdateStrategyDto input)
        {
            var strategy = await _strategyRepository.GetByIdAsync(input.Id);

            var existingStrategy = await _strategyRepository.GetByNameAsync(input.Name);

            if (existingStrategy is not null && existingStrategy.Id != strategy.Id)
            {
                throw new StrategyAlreadyExistsException(existingStrategy.Name);
            }

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

            strategy.Update(
                input.Name,
                input.Description,
                rules);

            await _strategyRepository.UpdateAsync(strategy);
        }

        /// <inheritdoc />
        public Task<List<RuleDto>> GetAvailableRulesAsync()
        {
            var ruleContributorType = typeof(IRuleContributor);
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

            return Task.FromResult(rules);
        }
    }
}
