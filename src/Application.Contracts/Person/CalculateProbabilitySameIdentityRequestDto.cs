using System;

namespace Application.Contracts.Person
{
    public record CalculateProbabilitySameIdentityRequestDto
    {
        public Guid FirstPersonId { get; set; }

        public Guid SecondPersonId { get; set; }

        public Guid StrategyId { get; set; }
    }
}
