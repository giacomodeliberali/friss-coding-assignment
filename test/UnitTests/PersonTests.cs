using System;
using Domain.Model;
using Shouldly;
using WriteModel;
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

            // Act & Assert
            sut.Id.ShouldNotBe(Guid.Empty);
            sut.Snapshot.ShouldBeNull();
            sut.FirstName.ShouldBe("Giacomo");
            sut.LastName.ShouldBe("De Liberali");
            sut.BirthDate.ShouldBe(DateTime.Parse("01-01-1996"));
            sut.IdentificationNumber.ShouldBe("12345");
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
            var sut = Person.Factory.FromSnapshot(new PersonData()
            {
                Id = guid,
                FirstName = "Giacomo",
                LastName = "De Liberali",
                BirthDate = DateTime.Parse("01-01-1996"),
                IdentificationNumber = "12345",
            });

            // Act & Assert
            sut.Id.ShouldBe(guid);
            sut.FirstName.ShouldBe("Giacomo");
            sut.LastName.ShouldBe("De Liberali");
            sut.BirthDate.ShouldBe(DateTime.Parse("01-01-1996"));
            sut.IdentificationNumber.ShouldBe("12345");
        }
    }
}
