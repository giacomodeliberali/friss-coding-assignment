using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Rules;
using Domain.Model;
using Domain.Repositories;
using Domain.Rules;

namespace Application.Services
{
    public class PersonStrategyMatchApplicationService : IPersonStrategyMatchApplicationService
    {
        private readonly IPersonMatchingRuleFactory _personMatchingRuleFactory;
        private readonly IPersonMatchingStrategyRepository _strategyRepository;

        public PersonStrategyMatchApplicationService(
            IPersonMatchingRuleFactory personMatchingRuleFactory,
            IPersonMatchingStrategyRepository strategyRepository)
        {
            _personMatchingRuleFactory = personMatchingRuleFactory;
            _strategyRepository = strategyRepository;
        }

        public async Task CreateStrategy(CreateStrategyDto input)
        {
            var rules = new List<PersonMatchingRule>();

            foreach (var ruleDto in input.Rules)
            {
                var rule = _personMatchingRuleFactory.GetInstance(
                    ruleDto.RuleTypeAssemblyQualifiedName,
                    ruleDto.Name,
                    ruleDto.Description);

                foreach (var parameterDto in ruleDto.Parameters ?? Enumerable.Empty<CreateStrategyDto.CreateRuleDto.CreateRuleParameterDto>())
                {
                    var parameter = MatchingRuleParameter.Factory.Create(parameterDto.Name, parameterDto.Value);
                    rule.AddParameter(parameter);
                }

                rules.Add(rule);
            }

            var strategy = PersonMatchingStrategy.Factory.Create(
                input.Name,
                input.Description,
                rules);

            await _strategyRepository.CreateAsync(strategy);
        }

        public async Task<StrategyDto> GetByIdAsync(Guid strategyId)
        {
            var strategy = await _strategyRepository.GetByIdAsync(strategyId);

            if (strategy == null)
            {
                return null;
            }

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
                        }).ToList(),
                    };
                }).ToList(),
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
