using System;
using WriteModel;

namespace Domain.Model
{
    public class MatchingRuleParameter : Entity
    {
        private PersonMatchingRuleParameterData _snapshot;

        public PersonMatchingRuleParameterData Snapshot => _snapshot;

        public string Name { get; set; }

        public decimal Value { get; set; }

        private MatchingRuleParameter()
        {
        }

        public static class Factory
        {
            public static MatchingRuleParameter Create(string name, decimal value)
            {
                return new MatchingRuleParameter()
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Value = value,
                };
            }

            public static MatchingRuleParameter FromSnapshot(PersonMatchingRuleParameterData snapshot)
            {
                return new MatchingRuleParameter()
                {
                    Id = snapshot.Id,
                    Name = snapshot.Name,
                    Value = snapshot.Value,
                    _snapshot = snapshot,
                };
            }
        }
    }
}
