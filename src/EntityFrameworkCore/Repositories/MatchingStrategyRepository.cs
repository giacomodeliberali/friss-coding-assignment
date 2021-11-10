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
    public class MatchingStrategyRepository : IMatchingStrategyRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public MatchingStrategyRepository(
            ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Guid> CreateAsync(MatchingStrategy strategy)
        {
            await CreateStrategyInternal(strategy);
            await CreateMatchingRulesInternal(strategy);

            await _applicationDbContext.SaveChangesAsync();

            return strategy.Id;
        }

        private async Task CreateStrategyInternal(MatchingStrategy strategy)
        {
            var strategyData = new MatchingStrategyData()
            {
                Id = strategy.Id,
                Name = strategy.Name,
                Description = strategy.Description,
            };

            await _applicationDbContext.PersonMatchingStrategies.AddAsync(strategyData);
        }

        private async Task CreateMatchingRulesInternal(MatchingStrategy strategy)
        {
            var rulesData = new List<MatchingRuleData>();
            var rulesParameterData = new List<MatchingRuleParameterData>();

            var order = 10;
            foreach (var personMatchingRule in strategy.Rules)
            {
                var ruleData = new MatchingRuleData()
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
                    var ruleParameterData = new MatchingRuleParameterData()
                    {
                        Id = ruleParameter.Id,
                        Name = ruleParameter.Name,
                        Value = ruleParameter.Value,
                        RuleId = personMatchingRule.Id,
                    };

                    rulesParameterData.Add(ruleParameterData);
                }
            }

            await _applicationDbContext.PersonMatchingRules.AddRangeAsync(rulesData);
            await _applicationDbContext.PersonMatchingRulesParameters.AddRangeAsync(rulesParameterData);
        }

        public async Task UpdateAsync(MatchingStrategy strategy)
        {
            // delete rules and parameters
            var rulesData = await _applicationDbContext.PersonMatchingRules.Where(r => r.StrategyId == strategy.Id).ToListAsync();
            var rulesIds = rulesData.Select(r => r.Id).ToList();
            var parametersData =await _applicationDbContext.PersonMatchingRulesParameters.Where(p => rulesIds.Contains(p.RuleId)).ToListAsync();

            _applicationDbContext.PersonMatchingRules.RemoveRange(rulesData);
            _applicationDbContext.PersonMatchingRulesParameters.RemoveRange(parametersData);

            await CreateMatchingRulesInternal(strategy);

            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(MatchingStrategy strategy)
        {
            _applicationDbContext.PersonMatchingStrategies.Remove(strategy.Snapshot);

            // delete rules and parameters
            _applicationDbContext.PersonMatchingRules.RemoveRange(strategy.Rules.Select(r => r.Snapshot));
            _applicationDbContext.PersonMatchingRulesParameters.RemoveRange(strategy.Rules.SelectMany(r => r.Parameters.Select(p => p.Snapshot)));

            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<MatchingStrategy> GetByIdAsync(Guid id)
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

            var strategy = MatchingStrategy.Factory.FromSnapshot(
                strategyData,
                rulesData,
                parametersData);

            return strategy;
        }

        public async Task<MatchingStrategy> GetByNameAsync(string name)
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
