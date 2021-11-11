using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Exceptions;
using Domain.Extensions;
using Domain.Model;
using Domain.Rules;

namespace Application.Rules
{
    /// <inheritdoc />
    public class MatchingRuleStrategyExecutor : IMatchingRuleStrategyExecutor
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Creates the executor.
        /// </summary>
        /// <param name="serviceProvider">The DI container for resolving the rule types.</param>
        public MatchingRuleStrategyExecutor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public async Task<decimal> ExecuteAsync(MatchingStrategy strategy, Person first, Person second)
        {
            strategy.ThrowIfNull(nameof(strategy));
            first.ThrowIfNull(nameof(first));
            second.ThrowIfNull(nameof(second));

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
                        return async (probability) =>
                        {
                            if (probability >= MatchingProbabilityConstants.Match)
                            {
                                // interrupt the pipeline and return 100% match
                                return MatchingProbabilityConstants.Match;
                            }

                            if (currentRule.IsEnabled)
                            {
                                var executor = (IRuleContributor)_serviceProvider.GetService(currentRule.RuleType);

                                if (executor is null)
                                {
                                    throw new MatchingRuleNotRegisteredInDiContainer(currentRule.RuleType.FullName);
                                }

                                var newProbability = await executor.MatchAsync(currentRule, first, second, probability, next);

                                return newProbability;
                            }

                            return await next(probability);
                        };
                    });

            var initialScore = 0m;
            return await rulesPipelines(initialScore);
        }
    }
}
