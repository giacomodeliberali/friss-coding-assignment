using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Rules;
using Domain.Extensions;
using Domain.Model;
using Domain.Rules;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace UnitTests.Rules
{
    public class BirthDateEqualsMatchingRuleTest : RuleBaseTest
    {
        private readonly BirthDateEqualsMatchingRule _sut;

        public BirthDateEqualsMatchingRuleTest()
        {
            var logger = Substitute.For<ILogger<BirthDateEqualsMatchingRule>>();

            _sut = new BirthDateEqualsMatchingRule(logger);
        }

        [Fact]
        public async Task ShouldReturn_40_WhenBirthDatesMatch_UsingDefaultParameters()
        {
            // Arrange
            var person1 = Person.Factory.Create(
                "John",
                "Doe",
                DateTime.Parse("2000-01-01T10:00Z"),
                null);

            var person2 = Person.Factory.Create(
                "John",
                "Doe",
                DateTime.Parse("2000-01-01T22:00Z"),
                null);

            var rule = MatchingRule.Factory.Create(
                _sut.GetType().GetAssemblyQualifiedName(),
                "name",
                "description",
                isEnabled: true,
                new List<MatchingRuleParameter>());

            // Act
            var probability = await _sut.MatchAsync(rule, person1, person2, MatchingProbabilityConstants.NoMatch, NextMatchingRuleDelegate);

            // Assert
            probability.ShouldBe(0.4m);
            NextMatchingRuleDelegate.ReceivedCalls().Count().ShouldBe(1);
        }

        [Fact]
        public async Task ShouldReturn_UsingProvidedParameters_WhenBirthDatesMatch()
        {
            // Arrange
            var person1 = Person.Factory.Create(
                "John",
                "McAfee",
                DateTime.Parse("2000-01-01T10:00Z"),
                null);

            var person2 = Person.Factory.Create(
                "John",
                "McAfee",
                DateTime.Parse("2000-01-01T22:00Z"),
                null);

            var providedMatchProbability = 0.099m;

            var rule = MatchingRule.Factory.Create(
                _sut.GetType().GetAssemblyQualifiedName(),
                "name",
                "description",
                isEnabled: true,
                new List<MatchingRuleParameter>()
                {
                    MatchingRuleParameter.Factory.Create(BirthDateEqualsMatchingRule.IncreaseProbabilityWhenBirthDateMatches, providedMatchProbability),
                });

            // Act
            var probability = await _sut.MatchAsync(rule, person1, person2, MatchingProbabilityConstants.NoMatch, NextMatchingRuleDelegate);

            // Assert
            probability.ShouldBe(providedMatchProbability);
            NextMatchingRuleDelegate.ReceivedCalls().Count().ShouldBe(1);
        }

        [Fact]
        public async Task Should_Return_NoMatch_WhenBirthDatesAreKnownAndDifferent()
        {
            // Arrange
            var person1 = Person.Factory.Create(
                "John",
                "Doe",
                DateTime.Parse("2000-01-01T10:00Z"),
                null);

            var person2 = Person.Factory.Create(
                "John",
                "Doe",
                DateTime.Parse("2001-01-01T22:00Z"),
                null);

            var rule = MatchingRule.Factory.Create(
                _sut.GetType().GetAssemblyQualifiedName(),
                "name",
                "description",
                isEnabled: true,
                new List<MatchingRuleParameter>());

            // Act
            var probability = await _sut.MatchAsync(rule, person1, person2, MatchingProbabilityConstants.NoMatch, NextMatchingRuleDelegate);

            // Assert
            probability.ShouldBe(MatchingProbabilityConstants.NoMatch);
            NextMatchingRuleDelegate.ReceivedCalls().Count().ShouldBe(0);
        }
    }
}
