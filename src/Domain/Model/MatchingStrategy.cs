using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using WriteModel;

namespace Domain.Model
{
    public class MatchingStrategy : Entity
    {
        public MatchingStrategyData Snapshot => _snapshot;
        private MatchingStrategyData _snapshot;

        private List<MatchingRule> _rules;


        public string Name { get; protected set; }

        public string Description { get; protected set; }

        public IReadOnlyCollection<MatchingRule> Rules => _rules;

        private MatchingStrategy()
        {
        }

        public class Factory
        {
            public static MatchingStrategy Create(
                string name,
                string description,
                List<MatchingRule> rules)
            {
                Guard.Against.NullOrEmpty(name, nameof(name));
                Guard.Against.NullOrEmpty(description, nameof(description));
                Guard.Against.Null(rules, nameof(rules)); // does a strategy without rules make sense?

                return new MatchingStrategy()
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Description = description,
                    _rules = rules,
                };
            }

            public static MatchingStrategy FromSnapshot(
                MatchingStrategyData snapshot,
                List<MatchingRuleData> rulesSnapshot,
                List<MatchingRuleParameterData> parametersSnapshots)
            {
                return new MatchingStrategy()
                {
                    Id = snapshot.Id,
                    Name = snapshot.Name,
                    Description = snapshot.Description,
                    _snapshot = snapshot,
                    _rules = rulesSnapshot.Select(r =>
                    {
                        return MatchingRule.Factory.FromSnapshot(r, parametersSnapshots.Where(p => p.RuleId == r.Id).ToList());
                    }).ToList(),
                };
            }
        }
    }
}
