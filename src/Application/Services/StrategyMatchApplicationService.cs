using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Rules;
using Domain.Model;
using Domain.Repositories;
using Domain;

namespace Application.Services
{
    public class StrategyMatchApplicationService : IStrategyMatchApplicationService
    {
        private readonly IMatchingStrategyRepository _strategyRepository;

        public StrategyMatchApplicationService(
            IMatchingStrategyRepository strategyRepository)
        {
            _strategyRepository = strategyRepository;
        }

        public async Task<Guid> CreateStrategy(CreateStrategyDto input)
        {
            var rules = new List<MatchingRule>();

            foreach (var ruleDto in input.Rules.OrEmptyIfNull())
            {
                var parameters = new List<MatchingRuleParameter>();
                foreach (var parameterDto in ruleDto.Parameters.OrEmptyIfNull())
                {
                    var parameter = MatchingRuleParameter.Factory.Create(parameterDto.Name, parameterDto.Value);
                    parameters.Add(parameter);
                }

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
                throw new Exception($"Strategy with name '{strategy.Name}' already exists!");
            }

            await _strategyRepository.CreateAsync(strategy);

            return strategy.Id;
        }

        public async Task<StrategyDto> GetByIdAsync(Guid strategyId)
        {
            var strategy = await _strategyRepository.GetByIdAsync(strategyId);

            if (strategy == null)
            {
                return null;
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
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        RuleTypeAssemblyQualifiedName = r.GetType().FullName,
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

        public async Task DeleteStrategy(Guid id)
        {
            var strategy = await _strategyRepository.GetByIdAsync(id);

            if (strategy == null)
            {
                return;
            }

            await _strategyRepository.DeleteAsync(strategy);
        }
    }
}
