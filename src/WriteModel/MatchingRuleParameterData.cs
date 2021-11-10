using System;

namespace WriteModel
{
    public class MatchingRuleParameterData : EntityData
    {
        public string Name { get; set; }

        public decimal Value { get; set; }

        public Guid RuleId { get; set; }
    }
}
