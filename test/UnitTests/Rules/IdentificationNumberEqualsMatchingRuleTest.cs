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
    public class IdentificationNumberEqualsMatchingRuleTest : RuleBaseTest
    {
        private readonly IdentificationNumberEqualsMatchingRule _sut;

        public IdentificationNumberEqualsMatchingRuleTest()
        {
            var logger = Substitute.For<ILogger<IdentificationNumberEqualsMatchingRule>>();

            _sut = new IdentificationNumberEqualsMatchingRule(logger);
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
            var probabilitySameIdentity = await _sut.MatchAsync(rule, person1, person2, new ProbabilitySameIdentity(), NextMatchingRuleDelegate);

            // Assert
            probabilitySameIdentity.Probability.ShouldBe(ProbabilitySameIdentity.Match);
            probabilitySameIdentity.IsMatch().ShouldBe(true);
            probabilitySameIdentity.Contributors.Count.ShouldBe(1);
            NextMatchingRuleDelegate.ReceivedCalls().Count().ShouldBe(0); // should interrupt the pipeline by not calling the next
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

            var initialProbability = new ProbabilitySameIdentity(0.5m);

            // Act
            var probabilitySameIdentity = await _sut.MatchAsync(rule, person1, person2, initialProbability, NextMatchingRuleDelegate);

            // Assert
            probabilitySameIdentity.Probability.ShouldBe(0.5m);
            probabilitySameIdentity.Contributors.Count.ShouldBe(0);
            NextMatchingRuleDelegate.ReceivedCalls().Count().ShouldBe(1);
        }
    }
}
