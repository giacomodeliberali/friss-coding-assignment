using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Exceptions;
using Domain.Extensions;
using Domain.Model;
using Domain.Rules;
using Microsoft.Extensions.Logging;

namespace Application.Rules
{
    /// <inheritdoc />
    public class MatchingRuleStrategyExecutor : IMatchingRuleStrategyExecutor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MatchingRuleStrategyExecutor> _logger;

        /// <summary>
        /// Creates the executor.
        /// </summary>
        public MatchingRuleStrategyExecutor(
            IServiceProvider serviceProvider,
            ILogger<MatchingRuleStrategyExecutor> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<decimal> ExecuteAsync(MatchingStrategy strategy, Person first, Person second)
        {
            strategy.ThrowIfNull(nameof(strategy));
            first.ThrowIfNull(nameof(first));
            second.ThrowIfNull(nameof(second));

            NextMatchingRuleDelegate finalNext = (finalScore) =>
            {
                _logger.LogDebug("Pipeline terminated all the rules");
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
                            _logger.LogDebug("Executing rule {Rule} with type {RuleType}. Current probability = {Probability}", currentRule.Name, currentRule.RuleType.GetAssemblyQualifiedName(), probability);

                            if (probability >= MatchingProbabilityConstants.Match)
                            {
                                // interrupt the pipeline and return 100% match
                                _logger.LogDebug("Probability {Probability} >= 100%, exiting pipeline", probability);
                                return MatchingProbabilityConstants.Match;
                            }

                            if (currentRule.IsEnabled)
                            {
                                var executor = (IRuleContributor)_serviceProvider.GetService(currentRule.RuleType);

                                if (executor is null)
                                {
                                    throw new MatchingRuleNotRegisteredInDiContainer(currentRule.RuleType.FullName);
                                }

                                return await executor.MatchAsync(currentRule, first, second, probability, next);
                            }

                            return await next(probability);
                        };
                    });

            _logger.LogDebug("Start pipeline for strategy {StrategyName} comparing {FirstPersonId} and {SecondPersonId}", strategy.Name, first.Id, second.Id);

            const decimal initialScore = MatchingProbabilityConstants.NoMatch;
            var finalProbability =  await rulesPipelines(initialScore);

            _logger.LogDebug("End pipeline with probability {FinalProbability}", finalProbability);

            return finalProbability;
        }
    }
}
