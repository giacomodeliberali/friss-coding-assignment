using System.Collections.Generic;

namespace Application.Contracts.Person
{
    public record ProbabilitySameIdentityDto
    {
        public decimal Probability { get; set; }

        public List<ContributorDto> Contributors { get; set; }

        public record ContributorDto
        {
            public string Name { get; set; }

            public string Description { get; set; }

            public string RuleType { get; set; }

            public decimal Value { get; set; }
        }
    }
}
