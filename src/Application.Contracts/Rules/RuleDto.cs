using System.Collections.Generic;

namespace Application.Contracts.Rules
{
    public record RuleDto
    {
        public string AssemblyQualifiedName { get; set; }

        public string Description { get; set; }

        public List<ParameterDto> Parameters { get; set; }

        public record ParameterDto
        {
            public string Name { get; set; }

            public string Description { get; set; }
        }
    }
}
