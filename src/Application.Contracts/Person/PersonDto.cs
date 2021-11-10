using System;

namespace Application.Contracts.Person
{
    public record PersonDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string IdentificationNumber { get; set; }
    }
}
