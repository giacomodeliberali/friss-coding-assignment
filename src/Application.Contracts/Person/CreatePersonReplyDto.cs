using System;

namespace Application.Contracts.Person
{
    public record CreatePersonReplyDto
    {
        public Guid Id { get; set; }
    }
}
