using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Model;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using WriteModel;

namespace EntityFrameworkCore.Repositories
{
    public class PersonMatchingStrategyRepository : IPersonMatchingStrategyRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public PersonMatchingStrategyRepository(
            ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
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

            var order = 10;
            foreach (var personMatchingRule in strategy.Rules)
            {
                var ruleData = new PersonMatchingRuleData()
                {
                    Id = personMatchingRule.Id,
                    Name = personMatchingRule.Name,
                    Description = personMatchingRule.Description,
                    IsEnabled = personMatchingRule.IsEnabled,
                    Order = order,
                    RuleTypeAssemblyQualifiedName = personMatchingRule.RuleType.GetAssemblyQualifiedName(),
                    StrategyId = strategy.Id,
                };

                order += 10;

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
            _applicationDbContext.PersonMatchingStrategies.Remove(strategy.Snapshot);
            _applicationDbContext.PersonMatchingRules.RemoveRange(strategy.Rules.Select(r => r.Snapshot));
            _applicationDbContext.PersonMatchingRulesParameters.RemoveRange(strategy.Rules.SelectMany(r => r.Parameters.Select(p => p.Snapshot)));

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

            var strategy = PersonMatchingStrategy.Factory.FromSnapshot(
                strategyData,
                rulesData,
                parametersData);

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
