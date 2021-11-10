using System;
using System.Collections.Generic;

namespace Application.Contracts.Rules
{
    public record UpdateStrategyDto
    {
        public Guid Id { get; set; }

        public string Name { get; init; }

        public string Description { get; init; }

        public List<UpdateRuleDto> Rules { get; init; }

        public record UpdateRuleDto
        {
            public string Name { get; init; }

            public string Description { get; init; }

            public bool IsEnabled { get; set; }

            public string RuleTypeAssemblyQualifiedName { get; init; }

            public List<UpdateRuleParameterDto> Parameters { get; init; }

            public record UpdateRuleParameterDto
            {
                public string Name { get; init; }

                public string Description { get; init; }

                public decimal Value { get; init; }
            }
        }
    }
}
