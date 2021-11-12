using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Rules;
using Domain.Exceptions;
using Domain.Extensions;
using Domain.Model;
using Domain.Rules;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace UnitTests
{
    public class MatchingRuleStrategyExecutorTest
    {
        private readonly MatchingRuleStrategyExecutor _sut;
        private readonly Person _person1;
        private readonly Person _person2;

        public MatchingRuleStrategyExecutorTest()
        {
            var logger = Substitute.For<ILogger<MatchingRuleStrategyExecutor>>();
            var serviceProvider = Substitute.For<IServiceProvider>();

            // mock rules
            serviceProvider.GetService(typeof(AlwaysMatchingAndReturnRule)).Returns(new AlwaysMatchingAndReturnRule());
            serviceProvider.GetService(typeof(AlwaysNonMatchAndReturnRule)).Returns(new AlwaysNonMatchAndReturnRule());
            serviceProvider.GetService(typeof(AlwaysAdd50Rule)).Returns(new AlwaysAdd50Rule());

            _sut = new MatchingRuleStrategyExecutor(serviceProvider, logger);

            _person1 = Person.Factory.Create(
                "Andrew",
                "Doe",
                null,
                null);

            _person2 = Person.Factory.Create(
                "Andy",
                "McAfee",
                null,
                null);
        }

        [Fact]
        public async Task ShouldExecuteTheStrategy()
        {
            // Arrange
            var strategy = MatchingStrategy.Factory.Create(
                "Demo strategy",
                "description",
                new List<MatchingRule>()
                {
                    MatchingRule.Factory.Create(
                        typeof(AlwaysMatchingAndReturnRule).GetAssemblyQualifiedName(),
                        nameof(AlwaysMatchingAndReturnRule),
                        "description",
                        isEnabled: true,
                        new List<MatchingRuleParameter>())
                });

            // Act
            var probability = await _sut.ExecuteAsync(strategy, _person1, _person2);

            // Assert
            probability.ShouldBe(MatchingProbabilityConstants.Match);
        }

        [Fact]
        public async Task ShouldExecuteStrategyRules_InTheCorrectOrder()
        {
            // Arrange
            var strategy = MatchingStrategy.Factory.Create(
                "Demo strategy",
                "description",
                new List<MatchingRule>()
                {
                    MatchingRule.Factory.Create(
                        typeof(AlwaysMatchingAndReturnRule).GetAssemblyQualifiedName(),
                        nameof(AlwaysMatchingAndReturnRule),
                        "description",
                        isEnabled: true,
                        new List<MatchingRuleParameter>()),
                    MatchingRule.Factory.Create(
                        typeof(AlwaysNonMatchAndReturnRule).GetAssemblyQualifiedName(),
                        nameof(AlwaysNonMatchAndReturnRule),
                        "description",
                        isEnabled: true,
                        new List<MatchingRuleParameter>())
                });

            // Act
            var probability = await _sut.ExecuteAsync(strategy, _person1, _person2);

            // Assert
            probability.ShouldBe(MatchingProbabilityConstants.Match);
        }

        [Fact]
        public async Task Should_SkipNonEnabledRules()
        {
            // Arrange
            var strategy = MatchingStrategy.Factory.Create(
                "Demo strategy",
                "description",
                new List<MatchingRule>()
                {
                    MatchingRule.Factory.Create(
                        typeof(AlwaysMatchingAndReturnRule).GetAssemblyQualifiedName(),
                        nameof(AlwaysMatchingAndReturnRule),
                        "description",
                        isEnabled: false, // disabled rule
                        new List<MatchingRuleParameter>()),
                    MatchingRule.Factory.Create(
                        typeof(AlwaysNonMatchAndReturnRule).GetAssemblyQualifiedName(),
                        nameof(AlwaysNonMatchAndReturnRule),
                        "description",
                        isEnabled: true,
                        new List<MatchingRuleParameter>())
                });

            // Act
            var probability = await _sut.ExecuteAsync(strategy, _person1, _person2);

            // Assert
            probability.ShouldBe(MatchingProbabilityConstants.NoMatch);
        }

        [Fact]
        public async Task ShouldThrow_When_RuleIsNotRegisteredInDiContainer()
        {
            // Arrange
            var strategy = MatchingStrategy.Factory.Create(
                "Demo strategy",
                "description",
                new List<MatchingRule>()
                {
                    MatchingRule.Factory.Create(
                        typeof(RuleNonRegisteredInDiContainer).GetAssemblyQualifiedName(),
                        nameof(RuleNonRegisteredInDiContainer),
                        "description",
                        isEnabled: true,
                        new List<MatchingRuleParameter>()),
                });

            // Act & Assert
            await Should.ThrowAsync<MatchingRuleNotRegisteredInDiContainer>(async () =>
            {
                await _sut.ExecuteAsync(strategy, _person1, _person2);
            });
        }

        [Fact]
        public async Task Should_ReturnWhenProbabilityIsAbove100()
        {
            // Arrange
            var strategy = MatchingStrategy.Factory.Create(
                "Demo strategy",
                "description",
                new List<MatchingRule>()
                {
                    MatchingRule.Factory.Create(
                        typeof(AlwaysAdd50Rule).GetAssemblyQualifiedName(),
                        nameof(AlwaysAdd50Rule),
                        "description",
                        isEnabled: true,
                        new List<MatchingRuleParameter>()),
                    MatchingRule.Factory.Create(
                        typeof(AlwaysAdd50Rule).GetAssemblyQualifiedName(),
                        nameof(AlwaysAdd50Rule),
                        "description",
                        isEnabled: true,
                        new List<MatchingRuleParameter>()),
                    MatchingRule.Factory.Create(
                        typeof(AlwaysAdd50Rule).GetAssemblyQualifiedName(),
                        nameof(AlwaysAdd50Rule),
                        "description",
                        isEnabled: true,
                        new List<MatchingRuleParameter>()),
                });

            // Act
            var probability = await _sut.ExecuteAsync(strategy, _person1, _person2);

            // Assert
            probability.ShouldBe(MatchingProbabilityConstants.Match);
        }

        [Fact]
        public async Task Should_HandleAllDisabledRules()
        {
            // Arrange
            var strategy = MatchingStrategy.Factory.Create(
                "Demo strategy",
                "description",
                new List<MatchingRule>()
                {
                    MatchingRule.Factory.Create(
                        typeof(AlwaysAdd50Rule).GetAssemblyQualifiedName(),
                        nameof(AlwaysAdd50Rule),
                        "description",
                        isEnabled: false,
                        new List<MatchingRuleParameter>()),
                    MatchingRule.Factory.Create(
                        typeof(AlwaysAdd50Rule).GetAssemblyQualifiedName(),
                        nameof(AlwaysAdd50Rule),
                        "description",
                        isEnabled: false,
                        new List<MatchingRuleParameter>()),
                    MatchingRule.Factory.Create(
                        typeof(AlwaysAdd50Rule).GetAssemblyQualifiedName(),
                        nameof(AlwaysAdd50Rule),
                        "description",
                        isEnabled: false,
                        new List<MatchingRuleParameter>()),
                });

            // Act
            var probability = await _sut.ExecuteAsync(strategy, _person1, _person2);

            // Assert
            probability.ShouldBe(MatchingProbabilityConstants.NoMatch);
        }

        [Fact]
        public async Task Should_If_StrategyOrPeopleAreNull()
        {
            // Arrange
            var strategy = MatchingStrategy.Factory.Create(
                "Demo strategy",
                "description",
                new List<MatchingRule>()
                {
                    MatchingRule.Factory.Create(
                        typeof(AlwaysAdd50Rule).GetAssemblyQualifiedName(),
                        nameof(AlwaysAdd50Rule),
                        "description",
                        isEnabled: false,
                        new List<MatchingRuleParameter>()),
                });

            // Act & Assert
            await Should.ThrowAsync<ValidationException>(async () =>
            {
                await _sut.ExecuteAsync(null, _person1, _person2);
            });

            await Should.ThrowAsync<ValidationException>(async () =>
            {
                await _sut.ExecuteAsync(strategy, null, _person2);
            });

            await Should.ThrowAsync<ValidationException>(async () =>
            {
                await _sut.ExecuteAsync(strategy, _person1, null);
            });
        }

        // Mock rules

        public class AlwaysMatchingAndReturnRule : IRuleContributor
        {
            public async Task<decimal> MatchAsync(MatchingRule rule, Person first, Person second, decimal currentProbability,
                NextMatchingRuleDelegate next)
            {
                return MatchingProbabilityConstants.Match;
            }
        }

        public class AlwaysNonMatchAndReturnRule : IRuleContributor
        {
            public async Task<decimal> MatchAsync(MatchingRule rule, Person first, Person second, decimal currentProbability,
                NextMatchingRuleDelegate next)
            {
                return MatchingProbabilityConstants.NoMatch;
            }
        }

        public class RuleNonRegisteredInDiContainer : IRuleContributor
        {
            public async Task<decimal> MatchAsync(MatchingRule rule, Person first, Person second, decimal currentProbability,
                NextMatchingRuleDelegate next)
            {
                return MatchingProbabilityConstants.NoMatch;
            }
        }

        public class AlwaysAdd50Rule : IRuleContributor
        {
            public async Task<decimal> MatchAsync(MatchingRule rule, Person first, Person second, decimal currentProbability,
                NextMatchingRuleDelegate next)
            {
                return await next(currentProbability + 0.5m);
            }
        }
    }
}
