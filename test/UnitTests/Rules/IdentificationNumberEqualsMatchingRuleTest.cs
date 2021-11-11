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
    public class IdentificationNumberEqualsMatchingRuleTest
    {
        private readonly IdentificationNumberEqualsMatchingRule _sut;
        private readonly NextMatchingRuleDelegate _next;

        public IdentificationNumberEqualsMatchingRuleTest()
        {
            _sut = new IdentificationNumberEqualsMatchingRule();
            _next = Substitute.For<NextMatchingRuleDelegate>();
        }

        [Fact]
        public async Task ShouldReturn_MatchProbability_WhenIdentifiersMatch()
        {
            // Arrange
            const string identificationNumber = "12345";

            var person1 = Person.Factory.Create(
                "John",
                "Doe",
                null,
                identificationNumber);

            var person2 = Person.Factory.Create(
                "Alice",
                "McAfee",
                null,
                identificationNumber);

            var rule = MatchingRule.Factory.Create(
                _sut.GetType().GetAssemblyQualifiedName(),
                "name",
                "description",
                isEnabled: true,
                new List<MatchingRuleParameter>());

            // Act
            var probability = await _sut.MatchAsync(rule, person1, person2, MatchingProbabilityConstants.NoMatch, _next);

            // Assert
            probability.ShouldBe(MatchingProbabilityConstants.Match);
            _next.ReceivedCalls().Count().ShouldBe(0);
        }

        [Fact]
        public async Task ShouldCallNextDelegate_When_IdentifierDoNotMatch_AndReturnCurrentProbability()
        {
            // Arrange
            var person1 = Person.Factory.Create(
                "John",
                "Doe",
                null,
                "12345");

            var person2 = Person.Factory.Create(
                "Alice",
                "McAfee",
                null,
                "54321");

            var rule = MatchingRule.Factory.Create(
                _sut.GetType().GetAssemblyQualifiedName(),
                "name",
                "description",
                isEnabled: true,
                new List<MatchingRuleParameter>());

            const decimal initialProbability = 0.5m; // 50%

            _next.Invoke(Arg.Any<decimal>()).ReturnsForAnyArgs(x => Task.FromResult(x.Arg<decimal>())); // return input

            // Act
            var probability = await _sut.MatchAsync(rule, person1, person2, initialProbability, _next);

            // Assert
            probability.ShouldBe(initialProbability);
            _next.ReceivedCalls().Count().ShouldBe(1);
        }
    }
}
