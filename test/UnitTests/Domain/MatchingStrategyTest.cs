using System;
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
    public class MatchingStrategyTest
    {

        [Fact]
        public void Should_CreateAStrategy()
        {
            // Arrange
            var sut = MatchingStrategy.Factory.Create(
                "Name test",
                "Description test",
                new List<MatchingRule>()
                {
                    MatchingRule.Factory.Create(
                        typeof(IdentificationNumberEqualsMatchingRule).GetAssemblyQualifiedName(),
                        "Same identifiers",
                        "If same identifiers than 100% match",
                        isEnabled: true,
                        Enumerable.Empty<MatchingRuleParameter>().ToList()),
                });

            // Act & Assert
            sut.Id.ShouldNotBe(Guid.Empty);
            sut.Snapshot.ShouldBeNull();
            sut.Name.ShouldBe("Name test");
            sut.Description.ShouldBe("Description test");
            sut.Rules.Count.ShouldBe(1);
            sut.Rules.Single().Name.ShouldBe("Same identifiers");
            sut.Rules.Single().Description.ShouldBe("If same identifiers than 100% match");
            sut.Rules.Single().IsEnabled.ShouldBe(true);
            sut.Rules.Single().Parameters.Count.ShouldBe(0);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Should_Throw_WhenProvidingNameInvalidValues(string name)
        {
            // Arrange, Act & Assert
            Should.Throw<ValidationException>(() =>
            {
                var sut = MatchingStrategy.Factory.Create(
                    name,
                    "description",
                    new List<MatchingRule>());
            });
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Should_Throw_WhenProvidingDescriptionInvalidValues(string description)
        {
            // Arrange, Act & Assert
            Should.Throw<ValidationException>(() =>
            {
                var sut = MatchingStrategy.Factory.Create(
                    "name",
                    description,
                    new List<MatchingRule>());
            });
        }

        [Fact]
        public void Should_Throw_WhenProvidingNullRules()
        {
            // Arrange, Act & Assert
            Should.Throw<ValidationException>(() =>
            {
                var sut = MatchingStrategy.Factory.Create(
                    "name",
                    "description",
                    null);
            });
        }
    }
}
