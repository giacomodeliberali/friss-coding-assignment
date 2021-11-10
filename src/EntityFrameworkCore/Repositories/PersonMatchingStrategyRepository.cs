using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Repositories;
using Domain.Rules;
using EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Repositories
{
    public class PersonMatchingStrategyRepository : IPersonMatchingStrategyRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IPersonMatchingRuleFactory _personMatchingRuleFactory;

        public PersonMatchingStrategyRepository(
            ApplicationDbContext applicationDbContext,
            IPersonMatchingRuleFactory personMatchingRuleFactory)
        {
            _applicationDbContext = applicationDbContext;
            _personMatchingRuleFactory = personMatchingRuleFactory;
        }

        public async Task<Guid> CreateAsync(PersonMatchingStrategy strategy)
        {
            var strategyData = new PersonMatchingStrategyData()
            {
                Id = strategy.Id,
                Name = strategy.Name,
                Description = strategy.Description,
            };

            var rulesData = new List<PersonMatchingRuleData>();
            var rulesParameterData = new List<PersonMatchingRuleParameterData>();

            foreach (var personMatchingRule in strategy.Rules)
            {
                var ruleData = new PersonMatchingRuleData()
                {
                    Id = personMatchingRule.Id,
                    Name = personMatchingRule.Name,
                    Description = personMatchingRule.Description,
                    Order = 0,
                    RuleTypeAssemblyQualifiedName = personMatchingRule.GetType().AssemblyQualifiedName!,
                    StrategyId = strategy.Id,
                };

                rulesData.Add(ruleData);

                foreach (var ruleParameter in personMatchingRule.Parameters)
                {
                    var ruleParameterData = new PersonMatchingRuleParameterData()
                    {
                        Id = ruleParameter.Id,
                        Name = ruleParameter.Name,
                        Value = ruleParameter.Value,
                        RuleId = personMatchingRule.Id,
                    };

                    rulesParameterData.Add(ruleParameterData);
                }
            }

            await _applicationDbContext.PersonMatchingStrategies.AddAsync(strategyData);
            await _applicationDbContext.PersonMatchingRules.AddRangeAsync(rulesData);
            await _applicationDbContext.PersonMatchingRulesParameters.AddRangeAsync(rulesParameterData);

            await _applicationDbContext.SaveChangesAsync();

            return strategy.Id;
        }

        public Task UpdateAsync(PersonMatchingStrategy strategy)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(PersonMatchingStrategy strategy)
        {
            var strategyData = await _applicationDbContext.PersonMatchingStrategies
                .SingleAsync(s => s.Id == strategy.Id);

            var rulesData = await _applicationDbContext.PersonMatchingRules
                .Where(r => r.StrategyId == strategy.Id)
                .ToListAsync();

            var rulesId = rulesData.Select(r => r.Id).ToList();

            var parametersData = await _applicationDbContext.PersonMatchingRulesParameters
                .Where(p => rulesId.Contains(p.RuleId))
                .ToListAsync();

            _applicationDbContext.PersonMatchingStrategies.Remove(strategyData);
            _applicationDbContext.PersonMatchingRules.RemoveRange(rulesData);
            _applicationDbContext.PersonMatchingRulesParameters.RemoveRange(parametersData);

            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<PersonMatchingStrategy> GetByIdAsync(Guid id)
        {
            var strategyData = await _applicationDbContext.PersonMatchingStrategies
                .SingleOrDefaultAsync(s => s.Id == id);

            if (strategyData == null)
            {
                return null;
            }

            var rulesData = await _applicationDbContext.PersonMatchingRules
                .Where(r => r.StrategyId == id)
                .ToListAsync();

            var rulesId = rulesData.Select(r => r.Id).ToList();

            var parametersData = await _applicationDbContext.PersonMatchingRulesParameters
                .Where(p => rulesId.Contains(p.RuleId))
                .ToListAsync();

            // @GDL todo refactor!!!

            var strategy = PersonMatchingStrategy.Factory.FromSnapshot(
                strategyData.Id,
                strategyData.Name,
                strategyData.Description,
                rulesData.Select(r =>
                {
                    var rule = _personMatchingRuleFactory.GetInstance(r.RuleTypeAssemblyQualifiedName, r.Name,
                        r.Description, r.Id);

                    foreach (var parameterData in parametersData.Where(p => p.RuleId == r.Id))
                    {
                        rule.AddParameter(MatchingRuleParameter.Factory.FromSnapshot(parameterData.Id, parameterData.Name, parameterData.Value));
                    }

                    return rule;
                }).ToList());

            return strategy;
        }

        public async Task<PersonMatchingStrategy> GetByNameAsync(string name)
        {
            var strategyData = await _applicationDbContext.PersonMatchingStrategies.SingleOrDefaultAsync(s => s.Name == name);

            if (strategyData is null)
            {
                return null;
            }

            return await GetByIdAsync(strategyData.Id);
        }
    }
}
