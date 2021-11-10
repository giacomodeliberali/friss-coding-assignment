using System;

namespace Domain.Model
{
    public class MatchingRuleParameter : Entity
    {
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

            public static MatchingRuleParameter FromSnapshot(Guid id, string name, decimal value)
            {
                return new MatchingRuleParameter()
                {
                    Id = id,
                    Name = name,
                    Value = value,
                };
            }
        }
    }
}
