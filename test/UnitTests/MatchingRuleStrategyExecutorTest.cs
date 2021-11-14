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
            serviceProvider.GetService(typeof(AlwaysMatchingAndReturnMatchingRule)).Returns(new AlwaysMatchingAndReturnMatchingRule());
            serviceProvider.GetService(typeof(AlwaysNonMatchAndReturnMatchingRule)).Returns(new AlwaysNonMatchAndReturnMatchingRule());
            serviceProvider.GetService(typeof(AlwaysAdd50MatchingRule)).Returns(new AlwaysAdd50MatchingRule());

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
                        typeof(AlwaysMatchingAndReturnMatchingRule).GetAssemblyQualifiedName(),
                        nameof(AlwaysMatchingAndReturnMatchingRule),
                        "description",
                        isEnabled: true,
                        new List<MatchingRuleParameter>())
                });

            // Act
            var probabilitySameIdentity = await _sut.ExecuteAsync(strategy, _person1, _person2);

            // Assert
            probabilitySameIdentity.Probability.ShouldBe(ProbabilitySameIdentity.Match);
            probabilitySameIdentity.Contributors.Count.ShouldBe(1);
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
                        typeof(AlwaysMatchingAndReturnMatchingRule).GetAssemblyQualifiedName(),
                        nameof(AlwaysMatchingAndReturnMatchingRule),
                        "description",
                        isEnabled: true,
                        new List<MatchingRuleParameter>()),
                    MatchingRule.Factory.Create(
                        typeof(AlwaysNonMatchAndReturnMatchingRule).GetAssemblyQualifiedName(),
                        nameof(AlwaysNonMatchAndReturnMatchingRule),
                        "description",
                        isEnabled: true,
                        new List<MatchingRuleParameter>())
                });

            // Act
            var probabilitySameIdentity = await _sut.ExecuteAsync(strategy, _person1, _person2);

            // Assert
            probabilitySameIdentity.Probability.ShouldBe(ProbabilitySameIdentity.Match);
            probabilitySameIdentity.Contributors.Count.ShouldBe(1);
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
                        typeof(AlwaysMatchingAndReturnMatchingRule).GetAssemblyQualifiedName(),
                        nameof(AlwaysMatchingAndReturnMatchingRule),
                        "description",
                        isEnabled: false, // disabled rule
                        new List<MatchingRuleParameter>()),
                    MatchingRule.Factory.Create(
                        typeof(AlwaysNonMatchAndReturnMatchingRule).GetAssemblyQualifiedName(),
                        nameof(AlwaysNonMatchAndReturnMatchingRule),
                        "description",
                        isEnabled: true,
                        new List<MatchingRuleParameter>())
                });

            // Act
            var probabilitySameIdentity = await _sut.ExecuteAsync(strategy, _person1, _person2);

            // Assert
            probabilitySameIdentity.Probability.ShouldBe(ProbabilitySameIdentity.NoMatch);
            probabilitySameIdentity.Contributors.Count.ShouldBe(1);        }

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
                        typeof(MatchingRuleNonRegisteredInDiContainer).GetAssemblyQualifiedName(),
                        nameof(MatchingRuleNonRegisteredInDiContainer),
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
                        typeof(AlwaysAdd50MatchingRule).GetAssemblyQualifiedName(),
                        nameof(AlwaysAdd50MatchingRule),
                        "description",
                        isEnabled: true,
                        new List<MatchingRuleParameter>()),
                    MatchingRule.Factory.Create(
                        typeof(AlwaysAdd50MatchingRule).GetAssemblyQualifiedName(),
                        nameof(AlwaysAdd50MatchingRule),
                        "description",
                        isEnabled: true,
                        new List<MatchingRuleParameter>()),
                    MatchingRule.Factory.Create(
                        typeof(AlwaysAdd50MatchingRule).GetAssemblyQualifiedName(),
                        nameof(AlwaysAdd50MatchingRule),
                        "description",
                        isEnabled: true,
                        new List<MatchingRuleParameter>()),
                });

            // Act
            var probabilitySameIdentity = await _sut.ExecuteAsync(strategy, _person1, _person2);

            // Assert
            probabilitySameIdentity.Probability.ShouldBe(ProbabilitySameIdentity.Match);
            probabilitySameIdentity.Contributors.Count.ShouldBe(2);
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
                        typeof(AlwaysAdd50MatchingRule).GetAssemblyQualifiedName(),
                        nameof(AlwaysAdd50MatchingRule),
                        "description",
                        isEnabled: false,
                        new List<MatchingRuleParameter>()),
                    MatchingRule.Factory.Create(
                        typeof(AlwaysAdd50MatchingRule).GetAssemblyQualifiedName(),
                        nameof(AlwaysAdd50MatchingRule),
                        "description",
                        isEnabled: false,
                        new List<MatchingRuleParameter>()),
                    MatchingRule.Factory.Create(
                        typeof(AlwaysAdd50MatchingRule).GetAssemblyQualifiedName(),
                        nameof(AlwaysAdd50MatchingRule),
                        "description",
                        isEnabled: false,
                        new List<MatchingRuleParameter>()),
                });

            // Act
            var probabilitySameIdentity = await _sut.ExecuteAsync(strategy, _person1, _person2);

            // Assert
            probabilitySameIdentity.Probability.ShouldBe(ProbabilitySameIdentity.NoMatch);
            probabilitySameIdentity.Contributors.Count.ShouldBe(0);
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
                        typeof(AlwaysAdd50MatchingRule).GetAssemblyQualifiedName(),
                        nameof(AlwaysAdd50MatchingRule),
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

        public class AlwaysMatchingAndReturnMatchingRule : IMatchingRuleContributor
        {
            public Task<ProbabilitySameIdentity> MatchAsync(MatchingRule rule, Person first, Person second, ProbabilitySameIdentity currentProbability,
                NextMatchingRuleDelegate next)
            {
                return Task.FromResult(currentProbability.SetMatch(rule));
            }
        }

        public class AlwaysNonMatchAndReturnMatchingRule : IMatchingRuleContributor
        {
            public Task<ProbabilitySameIdentity> MatchAsync(MatchingRule rule, Person first, Person second, ProbabilitySameIdentity currentProbability,
                NextMatchingRuleDelegate next)
            {
                return Task.FromResult(currentProbability.SetNoMatch(rule));
            }
        }

        public class MatchingRuleNonRegisteredInDiContainer : IMatchingRuleContributor
        {
            public Task<ProbabilitySameIdentity> MatchAsync(MatchingRule rule, Person first, Person second, ProbabilitySameIdentity currentProbability,
                NextMatchingRuleDelegate next)
            {
                return Task.FromResult(currentProbability.SetNoMatch(rule));
            }
        }

        public class AlwaysAdd50MatchingRule : IMatchingRuleContributor
        {
            public async Task<ProbabilitySameIdentity> MatchAsync(MatchingRule rule, Person first, Person second, ProbabilitySameIdentity currentProbability,
                NextMatchingRuleDelegate next)
            {
                currentProbability.AddContributor(rule, 0.5m);
                return await next(currentProbability);
            }
        }
    }
}
