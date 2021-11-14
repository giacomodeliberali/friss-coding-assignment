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
        private readonly FirstNameMatchingMatchingRule _sut;

        public FirstNameMatchingRuleTest()
        {
            var logger = Substitute.For<ILogger<FirstNameMatchingMatchingRule>>();

            _sut = new FirstNameMatchingMatchingRule(logger);
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
            var probabilitySameIdentity = await _sut.MatchAsync(rule, person1, person2, new ProbabilitySameIdentity(), NextMatchingRuleDelegate);

            // Assert
            probabilitySameIdentity.Probability.ShouldBe(0.2m);
            probabilitySameIdentity.Contributors.Count.ShouldBe(1);
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
            var probabilitySameIdentity = await _sut.MatchAsync(rule, person1, person2, new ProbabilitySameIdentity(), NextMatchingRuleDelegate);

            // Assert
            probabilitySameIdentity.Probability.ShouldBe(0.15m);
            probabilitySameIdentity.Contributors.Count.ShouldBe(1);
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
                    MatchingRuleParameter.Factory.Create(FirstNameMatchingMatchingRule.IncreaseProbabilityWhenEqualsFirstNames, providedMatchProbability),
                });

            // Act
            var probabilitySameIdentity = await _sut.MatchAsync(rule, person1, person2, new ProbabilitySameIdentity(), NextMatchingRuleDelegate);

            // Assert
            probabilitySameIdentity.Probability.ShouldBe(providedMatchProbability);
            probabilitySameIdentity.Contributors.Count.ShouldBe(1);
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
                    MatchingRuleParameter.Factory.Create(FirstNameMatchingMatchingRule.IncreaseProbabilityWhenSimilarFirstNames, providedSimilarityProbability),
                });

            // Act
            var probabilitySameIdentity = await _sut.MatchAsync(rule, person1, person2, new ProbabilitySameIdentity(), NextMatchingRuleDelegate);

            // Assert
            probabilitySameIdentity.Probability.ShouldBe(providedSimilarityProbability);
            probabilitySameIdentity.Contributors.Count.ShouldBe(1);
            NextMatchingRuleDelegate.ReceivedCalls().Count().ShouldBe(1);
        }
    }
}
