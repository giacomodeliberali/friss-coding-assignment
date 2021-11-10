using System;
using System.Collections.Generic;
using System.Linq;
using WriteModel;

namespace Domain.Model
{
    public class MatchingRule : Entity
    {
        public MatchingRuleData Snapshot => _snapshot;

        private MatchingRuleData _snapshot;

        private List<MatchingRuleParameter> _parameters;
        public string Name { get; internal set; }

        public string Description { get; internal set; }

        public bool IsEnabled { get; protected set; }

        public Type RuleType { get; private set; }

        public IReadOnlyCollection<MatchingRuleParameter> Parameters => _parameters;

        public decimal GetParameterOrDefault(string name, decimal defaultValue)
        {
            var parameter = Parameters.SingleOrDefault(p => p.Name == name);

            if (parameter is not null)
            {
                return parameter.Value;
            }

            return defaultValue;
        }

        public class Factory
        {
            public static MatchingRule Create(
                string ruleTypeAssemblyQualifiedName,
                string name,
                string description,
                bool isEnabled,
                List<MatchingRuleParameter> parameters)
            {
                var ruleType = Type.GetType(ruleTypeAssemblyQualifiedName);

                if (ruleType is null)
                {
                    throw new ArgumentException("Invalid rule type");
                }

                return new MatchingRule()
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Description = description,
                    IsEnabled = isEnabled,
                    _parameters = parameters,
                    RuleType = ruleType,
                };
            }

            public static MatchingRule FromSnapshot(
                MatchingRuleData snapshot,
                List<MatchingRuleParameterData> parameters)
            {
                var ruleType = Type.GetType(snapshot.RuleTypeAssemblyQualifiedName);

                if (ruleType is null)
                {
                    throw new ArgumentException("Invalid rule type");
                }

                return new MatchingRule()
                {
                    Id = snapshot.Id,
                    Name = snapshot.Name,
                    Description = snapshot.Description,
                    IsEnabled = snapshot.IsEnabled,
                    RuleType = ruleType,
                    _snapshot = snapshot,
                    _parameters = parameters.Select(MatchingRuleParameter.Factory.FromSnapshot).ToList(),
                };
            }
        }
    }
}
