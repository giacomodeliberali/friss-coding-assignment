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
    public class LastNameMatchingRuleTest : RuleBaseTest
    {
        private readonly LastNameMatchingRule _sut;

        public LastNameMatchingRuleTest()
        {
            var logger = Substitute.For<ILogger<LastNameMatchingRule>>();

            _sut = new LastNameMatchingRule(logger);
        }

        [Fact]
        public async Task ShouldReturn_40_WhenLastNamesMatch_UsingDefaultParameters()
        {
            // Arrange
            var person1 = Person.Factory.Create(
                "John",
                "Doe",
                null,
                null);

            var person2 = Person.Factory.Create(
                "John",
                "Doe",
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
            probabilitySameIdentity.Probability.ShouldBe(0.4m);
            probabilitySameIdentity.Contributors.Count.ShouldBe(1);
            NextMatchingRuleDelegate.ReceivedCalls().Count().ShouldBe(1);
        }

        [Fact]
        public async Task ShouldReturn_UsingProvidedParameters_WhenLastNamesMatch()
        {
            // Arrange
            var person1 = Person.Factory.Create(
                "John",
                "McAfee",
                null,
                null);

            var person2 = Person.Factory.Create(
                "John",
                "McAfee",
                null,
                null);

            var providedMatchProbability = 0.075m;

            var rule = MatchingRule.Factory.Create(
                _sut.GetType().GetAssemblyQualifiedName(),
                "name",
                "description",
                isEnabled: true,
                new List<MatchingRuleParameter>()
                {
                    MatchingRuleParameter.Factory.Create(LastNameMatchingRule.IncreaseProbabilityWhenEqualsLastNames, providedMatchProbability),
                });

            // Act
            var probabilitySameIdentity = await _sut.MatchAsync(rule, person1, person2, new ProbabilitySameIdentity(), NextMatchingRuleDelegate);

            // Assert
            probabilitySameIdentity.Probability.ShouldBe(providedMatchProbability);
            probabilitySameIdentity.Contributors.Count.ShouldBe(1);
            NextMatchingRuleDelegate.ReceivedCalls().Count().ShouldBe(1);
        }
    }
}
