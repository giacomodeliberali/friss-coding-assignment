using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Rules;
using Domain.Extensions;
using Domain.Model;
using Domain.Rules;
using NSubstitute;
using Shouldly;
using Xunit;

namespace UnitTests.Rules
{
    public class FirstNameMatchingRuleTest
    {
        private readonly FirstNameMatchingRule _sut;
        private readonly NextMatchingRuleDelegate _next;

        public FirstNameMatchingRuleTest()
        {
            _sut = new FirstNameMatchingRule();
            _next = Substitute.For<NextMatchingRuleDelegate>();
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

            _next.Invoke(Arg.Any<decimal>()).ReturnsForAnyArgs(x => Task.FromResult(x.Arg<decimal>())); // return input

            // Act
            var probability = await _sut.MatchAsync(rule, person1, person2, MatchingProbabilityConstants.NoMatch, _next);

            // Assert
            probability.ShouldBe(0.2m);
            _next.ReceivedCalls().Count().ShouldBe(1);
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

            _next.Invoke(Arg.Any<decimal>()).ReturnsForAnyArgs(x => Task.FromResult(x.Arg<decimal>())); // return input

            // Act
            var probability = await _sut.MatchAsync(rule, person1, person2, MatchingProbabilityConstants.NoMatch, _next);

            // Assert
            probability.ShouldBe(0.15m);
            _next.ReceivedCalls().Count().ShouldBe(1);
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

            _next.Invoke(Arg.Any<decimal>()).ReturnsForAnyArgs(x => Task.FromResult(x.Arg<decimal>())); // return input

            // Act
            var probability = await _sut.MatchAsync(rule, person1, person2, MatchingProbabilityConstants.NoMatch, _next);

            // Assert
            probability.ShouldBe(providedMatchProbability);
            _next.ReceivedCalls().Count().ShouldBe(1);
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

            _next.Invoke(Arg.Any<decimal>()).ReturnsForAnyArgs(x => Task.FromResult(x.Arg<decimal>())); // return input

            // Act
            var probability = await _sut.MatchAsync(rule, person1, person2, MatchingProbabilityConstants.NoMatch, _next);

            // Assert
            probability.ShouldBe(providedSimilarityProbability);
            _next.ReceivedCalls().Count().ShouldBe(1);
        }
    }
}
