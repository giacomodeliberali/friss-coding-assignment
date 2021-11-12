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
        public async Task<ProbabilitySameIdentity> ExecuteAsync(MatchingStrategy strategy, Person first, Person second)
        {
            strategy.ThrowIfNull(nameof(strategy));
            first.ThrowIfNull(nameof(first));
            second.ThrowIfNull(nameof(second));

            _logger.LogDebug("Start pipeline for strategy {StrategyName} comparing {FirstPersonId} and {SecondPersonId}", strategy.Name, first.Id, second.Id);

            if (first.Id == second.Id)
            {
                _logger.LogDebug("People have the same id, terminating pipeline");
                return new ProbabilitySameIdentity(MatchingProbabilityConstants.Match);
            }

            NextMatchingRuleDelegate finalNext = (finalScore) =>
            {
                _logger.LogDebug("Pipeline terminated all rules without interruption");
                return Task.FromResult(finalScore);
            };

            var rulesPipelines = strategy.Rules
                .Reverse()
                .Aggregate(
                    finalNext,
                    (next, currentRule) =>
                    {
                        return async (current) =>
                        {
                            _logger.LogDebug(
                                "Executing rule {Rule} with type {RuleType}. Current probability = {Probability}",
                                currentRule.Name,
                                currentRule.RuleType.GetAssemblyQualifiedName(),
                                current.Probability);

                            if (current.IsMatch())
                            {
                                // interrupt the pipeline
                                _logger.LogDebug("Probability is 100%, exiting pipeline");
                                return current;
                            }

                            if (currentRule.IsEnabled)
                            {
                                var ruleContributor = (IRuleContributor)_serviceProvider.GetService(currentRule.RuleType);

                                if (ruleContributor is null)
                                {
                                    throw new MatchingRuleNotRegisteredInDiContainer(currentRule.RuleType.FullName);
                                }

                                return await ruleContributor.MatchAsync(currentRule, first, second, current, next);
                            }

                            // skip this rule and continue the pipeline
                            return await next(current);
                        };
                    });

            var probabilitySameIdentity = new ProbabilitySameIdentity();

            await rulesPipelines(probabilitySameIdentity);

            _logger.LogDebug("End pipeline with probability {FinalProbability}", probabilitySameIdentity.Probability);

            return probabilitySameIdentity;
        }
    }
}
