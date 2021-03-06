using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Extensions;
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

        /// <inheritdoc />
        public async Task<Guid> CreateAsync(MatchingStrategy strategy)
        {
            await CreateStrategyInternal(strategy);
            await CreateMatchingRulesInternal(strategy);

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

            await _applicationDbContext.MatchingStrategies.AddAsync(strategyData);
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

            await _applicationDbContext.MatchingRules.AddRangeAsync(rulesData);
            await _applicationDbContext.MatchingRulesParameters.AddRangeAsync(rulesParameterData);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(MatchingStrategy strategy)
        {
            // delete rules and parameters
            var rulesData = await _applicationDbContext.MatchingRules.Where(r => r.StrategyId == strategy.Id).ToListAsync();
            var rulesIds = rulesData.Select(r => r.Id).ToList();
            var parametersData = await _applicationDbContext.MatchingRulesParameters.Where(p => rulesIds.Contains(p.RuleId)).ToListAsync();

            _applicationDbContext.MatchingRules.RemoveRange(rulesData);
            _applicationDbContext.MatchingRulesParameters.RemoveRange(parametersData);

            await CreateMatchingRulesInternal(strategy);
        }

        /// <inheritdoc />
        public Task DeleteAsync(MatchingStrategy strategy)
        {
            _applicationDbContext.MatchingStrategies.Remove(strategy.Snapshot);

            // delete rules and parameters
            _applicationDbContext.MatchingRules.RemoveRange(strategy.Rules.Select(r => r.Snapshot));
            _applicationDbContext.MatchingRulesParameters.RemoveRange(strategy.Rules.SelectMany(r => r.Parameters.Select(p => p.Snapshot)));

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task<MatchingStrategy> GetByIdAsync(Guid id)
        {
            var strategyData = await _applicationDbContext.MatchingStrategies
                .SingleOrDefaultAsync(s => s.Id == id);

            if (strategyData == null)
            {
                return null;
            }

            var rulesData = await _applicationDbContext.MatchingRules
                .Where(r => r.StrategyId == id)
                .OrderBy(r => r.Order)
                .ToListAsync();

            var rulesId = rulesData.Select(r => r.Id).ToList();

            var parametersData = await _applicationDbContext.MatchingRulesParameters
                .Where(p => rulesId.Contains(p.RuleId))
                .ToListAsync();

            var strategy = MatchingStrategy.Factory.FromSnapshot(
                strategyData,
                rulesData,
                parametersData);

            return strategy;
        }

        /// <inheritdoc />
        public async Task<List<MatchingStrategy>> GetAllAsync()
        {
            var result = new List<MatchingStrategy>();
            var strategiesData = await _applicationDbContext.MatchingStrategies.ToListAsync();

            // Note: bad approach, it performs multiple queries
            foreach (var strategyData in strategiesData)
            {
                result.Add(await GetByIdAsync(strategyData.Id));
            }

            return result;
        }

        /// <inheritdoc />
        public async Task SaveChangesAsync()
        {
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
