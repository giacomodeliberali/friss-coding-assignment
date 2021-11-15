using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Exceptions;
using Domain.Extensions;
using Domain.Model;
using Domain.Rules;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;
using SerilogTimings.Extensions;

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

            if (first.Id == second.Id)
            {
                _logger.LogDebug("People have the same id, terminating pipeline with 100% match");
                return new ProbabilitySameIdentity(initialProbability: ProbabilitySameIdentity.Match);
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
                                "Executing rule {RuleName} with type {RuleType}. Current probability = {Probability}",
                                currentRule.Name,
                                currentRule.RuleType.GetAssemblyQualifiedName(),
                                current.Probability);

                            if (current.IsMatch())
                            {
                                // interrupt the pipeline
                                _logger.LogInformation("Probability is 100%, interrupting pipeline");
                                return current;
                            }

                            if (currentRule.IsEnabled)
                            {
                                _logger.LogDebug(
                                    "Rule {RuleName} with type {RuleType} is enabled",
                                    currentRule.Name,
                                    currentRule.RuleType.GetAssemblyQualifiedName());

                                var ruleContributor = (IMatchingRuleContributor)_serviceProvider.GetService(currentRule.RuleType);

                                if (ruleContributor is null)
                                {
                                    _logger.LogError(
                                        "RuleType {RuleType} is not registered in DI container",
                                        currentRule.RuleType.GetAssemblyQualifiedName());

                                    throw new MatchingRuleNotRegisteredInDiContainer(currentRule.RuleType.FullName);
                                }

                                return await ruleContributor.MatchAsync(currentRule, first, second, current, next);
                            }

                            // skip this rule and continue the pipeline

                            _logger.LogDebug(
                                "Rule {RuleName} with type {RuleType} is disabled, skipping to next",
                                currentRule.Name,
                                currentRule.RuleType.GetAssemblyQualifiedName());

                            return await next(current);
                        };
                    });

            var probabilitySameIdentity = new ProbabilitySameIdentity();

            using (Log.ForContext<MatchingRuleStrategyExecutor>().TimeOperation("Matching rules pipeline execution"))
            {
                await rulesPipelines(probabilitySameIdentity);
            }

            _logger.LogDebug("End pipeline with probability {FinalProbability}", probabilitySameIdentity.Probability);

            return probabilitySameIdentity;
        }
    }
}
