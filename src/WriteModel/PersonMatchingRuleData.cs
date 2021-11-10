using System;

namespace WriteModel
{
    public class PersonMatchingRuleData : EntityData
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsEnabled { get; set; }

        public string RuleTypeAssemblyQualifiedName { get; set; }

        public Guid StrategyId { get; set; }

        public int Order { get; set; }
    }
}
