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
    public class FirstNameMatchingRuleTest : RuleBaseTest
    {
        private readonly FirstNameMatchingRule _sut;

        public FirstNameMatchingRuleTest()
        {
            var logger = Substitute.For<ILogger<FirstNameMatchingRule>>();

            _sut = new FirstNameMatchingRule(logger);
        }

        [Fact]
        public async Task ShouldReturn_20_WhenFirstNamesMatch_UsingDefaultParameters()
        {
            // Arrange
            var person1 = Person.Factory.Create(
                "John",
                "Doe",
                null,
                null);

            var person2 = Person.Factory.Create(
                "John",
                "McAfee",
                null,
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
            probability.ShouldBe(0.2m);
            NextMatchingRuleDelegate.ReceivedCalls().Count().ShouldBe(1);
        }

        [Theory]
        [InlineData("Andrew","Andy")]
        [InlineData("John","Johnny")]
        [InlineData("A.","Andrew")]
        [InlineData("Andrew", "A.")]
        [InlineData("Andrew", "  A. ")]
        [InlineData("  A. ", "Andrew")]
        public async Task ShouldReturn_15_WhenFirstNamesAreSimilar_UsingDefaultParameters(string first, string second)
        {
            // Arrange
            var person1 = Person.Factory.Create(
                first,
                "Doe",
                null,
                null);

            var person2 = Person.Factory.Create(
                second,
                "McAfee",
                null,
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
            probability.ShouldBe(0.15m);
            NextMatchingRuleDelegate.ReceivedCalls().Count().ShouldBe(1);
        }

        [Fact]
        public async Task ShouldReturn_UsingProvidedParameters_WhenFirstNamesMatch()
        {
            // Arrange
            var person1 = Person.Factory.Create(
                "John",
                "Doe",
                null,
                null);

            var person2 = Person.Factory.Create(
                "John",
                "McAfee",
                null,
                null);

            var providedMatchProbability = 0.05m;

            var rule = MatchingRule.Factory.Create(
                _sut.GetType().GetAssemblyQualifiedName(),
                "name",
                "description",
                isEnabled: true,
                new List<MatchingRuleParameter>()
                {
                    MatchingRuleParameter.Factory.Create(FirstNameMatchingRule.IncreaseProbabilityWhenEqualsFirstNames, providedMatchProbability),
                });

            // Act
            var probability = await _sut.MatchAsync(rule, person1, person2, MatchingProbabilityConstants.NoMatch, NextMatchingRuleDelegate);

            // Assert
            probability.ShouldBe(providedMatchProbability);
            NextMatchingRuleDelegate.ReceivedCalls().Count().ShouldBe(1);
        }

        [Fact]
        public async Task ShouldReturn_UsingDefaultParameters_WhenFirstNamesAreSimilar()
        {
            // Arrange
            var person1 = Person.Factory.Create(
                "Andrew",
                "Doe",
                null,
                null);

            var person2 = Person.Factory.Create(
                "Andy",
                "McAfee",
                null,
                null);

            var providedSimilarityProbability = 0.03m;

            var rule = MatchingRule.Factory.Create(
                _sut.GetType().GetAssemblyQualifiedName(),
                "name",
                "description",
                isEnabled: true,
                new List<MatchingRuleParameter>()
                {
                    MatchingRuleParameter.Factory.Create(FirstNameMatchingRule.IncreaseProbabilityWhenSimilarFirstNames, providedSimilarityProbability),
                });

            // Act
            var probability = await _sut.MatchAsync(rule, person1, person2, MatchingProbabilityConstants.NoMatch, NextMatchingRuleDelegate);

            // Assert
            probability.ShouldBe(providedSimilarityProbability);
            NextMatchingRuleDelegate.ReceivedCalls().Count().ShouldBe(1);
        }
    }
}
