using System;
using System.Collections.Generic;

namespace Application.Contracts
{
    public record StrategyDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<RuleDto> Rules { get; set; }

        public class RuleDto
        {
            public string Name { get; set; }

            public string Description { get; set; }

            public bool IsEnabled { get; set; }

            public string RuleTypeAssemblyQualifiedName { get; set; }

            public IEnumerable<ParameterDto> Parameters { get; set; }

            public class ParameterDto
            {
                public Guid Id { get; set; }

                public string Name { get; set; }

                public decimal Value { get; set; }
            }
        }
    }

}
