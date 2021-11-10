using System;

namespace EntityFrameworkCore.Entities
{
    /// <summary>
    /// Represents the write model of the <see cref="Person"/> domain entity.
    /// In this way the domain model and the SQL schema are decoupled.
    /// </summary>
    public class PersonData : EntityData
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string IdentificationNumber { get; set; }
    }
}
