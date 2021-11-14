using System.Collections.Generic;
using System.Linq;
using Application.Rules;
using Domain.Exceptions;
using Domain.Extensions;
using Domain.Model;
using Shouldly;
using Xunit;

namespace UnitTests.Domain
{
    public class MatchingRuleTest
    {
        [Fact]
        public void Should_Throw_WhenProviding_NonResolvableRuleType()
        {
            // Arrange, Act & Assert
            Should.Throw<InvalidRuleTypeException>(() =>
            {
                var sut = MatchingRule.Factory.Create(
                    "invalid assembly name",
                    "name",
                    "description",
                    isEnabled: true,
                    new List<MatchingRuleParameter>());
            });
        }

        [Fact]
        public void Should_Throw_WhenProviding_RuleTypeWhichDoesntImplement_IRuleContributor()
        {
            // Arrange, Act & Assert
            Should.Throw<InvalidRuleTypeException>(() =>
            {
                var sut = MatchingRule.Factory.Create(
                    typeof(MatchingRuleStrategyExecutor).GetAssemblyQualifiedName(),
                    "name",
                    "description",
                    isEnabled: true,
                    new List<MatchingRuleParameter>());
            });
        }

        [Fact]
        public void Should_Throw_WhenProviding_InvalidRuleParameter()
        {
            // Arrange, Act & Assert
            Should.Throw<InvalidParameterException>(() =>
            {
                var sut = MatchingRule.Factory.Create(
                    typeof(FirstNameMatchingMatchingRule).GetAssemblyQualifiedName(),
                    "name",
                    "description",
                    isEnabled: true,
                    new List<MatchingRuleParameter>()
                    {
                        MatchingRuleParameter.Factory.Create("NonExistingParameterName", 0.1m)
                    });
            });
        }

        [Fact]
        public void Should_CreateAMatchingRule()
        {
            // Arrange
            var sut = MatchingRule.Factory.Create(
                typeof(FirstNameMatchingMatchingRule).GetAssemblyQualifiedName(),
                "name",
                "description",
                isEnabled: false,
                new List<MatchingRuleParameter>()
                {
                    MatchingRuleParameter.Factory.Create(FirstNameMatchingMatchingRule.IncreaseProbabilityWhenEqualsFirstNames, 0.05m)
                });

            // Act & Assert
            sut.Name.ShouldBe("name");
            sut.Description.ShouldBe("description");
            sut.IsEnabled.ShouldBe(false);
            sut.RuleType.ShouldBe(typeof(FirstNameMatchingMatchingRule));
            sut.Parameters.Count.ShouldBe(1);
            sut.Parameters.Single().Name.ShouldBe(FirstNameMatchingMatchingRule.IncreaseProbabilityWhenEqualsFirstNames);
            sut.Parameters.Single().Value.ShouldBe(0.05m);
        }

        [Fact]
        public void Should_Throw_WhenPassingDuplicateParameters()
        {
            // Arrange, Act & Assert
            Should.Throw<DuplicatedParametersException>(() =>
            {
                var sut = MatchingRule.Factory.Create(
                    typeof(FirstNameMatchingMatchingRule).GetAssemblyQualifiedName(),
                    "name",
                    "description",
                    isEnabled: false,
                    new List<MatchingRuleParameter>()
                    {
                        MatchingRuleParameter.Factory.Create(FirstNameMatchingMatchingRule.IncreaseProbabilityWhenEqualsFirstNames, 0.05m),
                        MatchingRuleParameter.Factory.Create(FirstNameMatchingMatchingRule.IncreaseProbabilityWhenEqualsFirstNames, 0.08m),
                        MatchingRuleParameter.Factory.Create(FirstNameMatchingMatchingRule.IncreaseProbabilityWhenSimilarFirstNames, 0.08m),
                    });
            });
        }
    }
}
