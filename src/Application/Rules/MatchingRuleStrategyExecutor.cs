using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Rules
{
    public class MatchingRuleStrategyExecutor : IMatchingRuleStrategyExecutor
    {
        private readonly IServiceProvider _serviceProvider;

        public MatchingRuleStrategyExecutor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<decimal> ExecuteAsync(MatchingStrategy strategy, Person first, Person second)
        {
            NextMatchingRuleDelegate finalNext = (finalScore) =>
            {
                return Task.FromResult(finalScore);
            };

            var rulesPipelines = strategy.Rules
                .Reverse()
                .Aggregate(
                    finalNext,
                    (next, currentRule) =>
                    {
                        return async (score) =>
                        {
                            if (currentRule.IsEnabled)
                            {
                                var executor = (IRuleContributor)_serviceProvider.GetService(currentRule.RuleType);

                                if (executor == null)
                                {
                                    throw new Exception($"Type '{currentRule.RuleType.FullName}' is not registered in the DI container!");
                                }

                                var newScore = await executor.MatchAsync(currentRule, first, second, score, next);

                                return newScore;
                            }

                            return await next(score);
                        };
                    });

            var initialScore = 0m;
            return await rulesPipelines(initialScore);
        }
    }
}
