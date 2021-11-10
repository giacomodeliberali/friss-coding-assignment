using System;
using Domain.Model;
using Shouldly;
using Xunit;

namespace UnitTests
{
    public class PersonTests
    {

        [Fact]
        public void Should_CreateAPerson_AndHandleWhiteSpacesProperly()
        {
            // Arrange
            var sut = Person.Factory.Create(
                "Giacomo ",
                " De Liberali",
                DateTime.Parse("01-01-1996"),
                " 12345 ");

            var expected = Person.Factory.FromSnapshot(
                sut.Id,
                "Giacomo",
                "De Liberali",
                DateTime.Parse("01-01-1996"),
                "12345");

            // Act & Assert
            sut.ShouldBeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ShouldNot_AllowEmptyOrNullValuesForFirstName(string firstName)
        {
            // Arrange, Act & Assert
            Should.Throw<ArgumentException>(() =>
            {
                var sut = Person.Factory.Create(
                    firstName,
                    "De Liberali",
                    birthDate: null,
                    identificationNumber: null);
            });
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ShouldNot_AllowEmptyOrNullValuesForLastName(string lastName)
        {
            // Arrange, Act & Assert
            Should.Throw<ArgumentException>(() =>
            {
                var sut = Person.Factory.Create(
                    "Giacomo",
                    lastName,
                    birthDate: null,
                    identificationNumber: null);
            });
        }

        [Fact]
        public void Should_PopulateAllPropertiesFromSnapshot()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var sut = Person.Factory.FromSnapshot(
                guid,
                "Giacomo",
                "De Liberali",
                DateTime.Parse("01-01-1996"),
                "12345");

            // Act & Assert
            sut.Id.ShouldBe(guid);
            sut.FirstName.ShouldBe("Giacomo");
            sut.LastName.ShouldBe("De Liberali");
            sut.BirthDate.ShouldBe(DateTime.Parse("01-01-1996"));
            sut.IdentificationNumber.ShouldBe("12345");
        }
    }
}
