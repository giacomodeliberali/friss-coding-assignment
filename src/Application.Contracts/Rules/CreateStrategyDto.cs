using System.Collections.Generic;

namespace Application.Contracts.Rules
{
    public record CreateStrategyDto
    {
        public string Name { get; init; }

        public string Description { get; init; }

        public List<CreateRuleDto> Rules { get; init; }

        public record CreateRuleDto
        {
            public string Name { get; init; }

            public string Description { get; init; }

            public string RuleTypeAssemblyQualifiedName { get; init; }

            public List<CreateRuleParameterDto> Parameters { get; init; }

            public record CreateRuleParameterDto
            {
                public string Name { get; init; }

                public string Description { get; init; }

                public decimal Value { get; init; }
            }
        }
    }
}
