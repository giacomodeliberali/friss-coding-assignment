using Domain.Exceptions;
using Domain.Model;
using Shouldly;
using Xunit;

namespace UnitTests.Domain
{
    public class MatchingRuleParameterTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(0.5)]
        [InlineData(-1)]
        [InlineData(-0.5)]
        public void Should_CreateAMatchingRuleParameter(decimal value)
        {
            // Arrange
            var sut = MatchingRuleParameter.Factory.Create(
                "name",
                value);

            // Act & Assert
            sut.Name.ShouldBe("name");
            sut.Value.ShouldBe(value);
        }

        [Theory]
        [InlineData(1.5)]
        [InlineData(2)]
        [InlineData(100)]
        [InlineData(-1.5)]
        [InlineData(-100)]
        public void Should_ThrowWhenParameterValueIfBiggerThan_1(decimal value)
        {
            // Arrange, Act & Assert
            Should.Throw<ValidationException>(() =>
            {
                var sut = MatchingRuleParameter.Factory.Create(
                    "name",
                    value);
            });
        }
    }
}
