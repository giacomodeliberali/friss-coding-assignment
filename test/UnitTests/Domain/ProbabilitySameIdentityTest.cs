using System.Collections.Generic;
using Domain.Exceptions;
using Domain.Extensions;
using Domain.Model;
using Domain.Rules;
using Shouldly;
using Xunit;

namespace UnitTests.Domain
{
    public class ProbabilitySameIdentityTest
    {
        private readonly MatchingRule _matchingRule;

        public ProbabilitySameIdentityTest()
        {
            _matchingRule = MatchingRule.Factory.Create(
                typeof(MatchingRuleStrategyExecutorTest.AlwaysMatchingAndReturnRule).GetAssemblyQualifiedName(),
                "name",
                "description",
                isEnabled: true,
                new List<MatchingRuleParameter>());
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(2)]
        public void Should_Throw_WhenRangeOutsideZeroOne(decimal initialValue)
        {
            Should.Throw<ValidationException>(() =>
            {
                var sut = new ProbabilitySameIdentity(initialValue);
            });
        }

        [Fact]
        public void Should_BeNoMatch()
        {
            // Arrange
            var sut = new ProbabilitySameIdentity();

            // Act & Assert
            sut.Probability.ShouldBe(MatchingProbabilityConstants.NoMatch);
            sut.IsMatch().ShouldBe(false);
        }

        [Fact]
        public void Should_BeMatch()
        {
            // Arrange
            var sut = new ProbabilitySameIdentity(1);

            // Act & Assert
            sut.Probability.ShouldBe(MatchingProbabilityConstants.Match);
            sut.IsMatch().ShouldBe(true);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(0.5, 0.5)]
        [InlineData(-0.5, 0)]
        [InlineData(1.5, 1)]
        [InlineData(-1.5, 0)]
        public void Should_KeepProbabilityInRange(decimal value, decimal expectedResult)
        {
            // Arrange
            var sut = new ProbabilitySameIdentity();

            // Act
            sut.AddContributor(_matchingRule, value);

            // Assert
            sut.Contributors.Count.ShouldBe(1);
            sut.Probability.ShouldBe(expectedResult);
        }
    }
}
