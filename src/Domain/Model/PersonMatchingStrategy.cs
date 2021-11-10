using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using WriteModel;

namespace Domain.Model
{
    public class PersonMatchingStrategy : Entity
    {
        public PersonMatchingStrategyData Snapshot => _snapshot;
        private PersonMatchingStrategyData _snapshot;

        private List<PersonMatchingRule> _rules;


        public string Name { get; protected set; }

        public string Description { get; protected set; }

        public IReadOnlyCollection<PersonMatchingRule> Rules => _rules;

        private PersonMatchingStrategy()
        {
        }

        public class Factory
        {
            public static PersonMatchingStrategy Create(
                string name,
                string description,
                List<PersonMatchingRule> rules)
            {
                Guard.Against.NullOrEmpty(name, nameof(name));
                Guard.Against.NullOrEmpty(description, nameof(description));
                Guard.Against.Null(rules, nameof(rules)); // does a strategy without rules make sense?

                return new PersonMatchingStrategy()
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Description = description,
                    _rules = rules,
                };
            }

            public static PersonMatchingStrategy FromSnapshot(
                PersonMatchingStrategyData snapshot,
                List<PersonMatchingRuleData> rulesSnapshot,
                List<PersonMatchingRuleParameterData> parametersSnapshots)
            {
                return new PersonMatchingStrategy()
                {
                    Id = snapshot.Id,
                    Name = snapshot.Name,
                    Description = snapshot.Description,
                    _snapshot = snapshot,
                    _rules = rulesSnapshot.Select(r =>
                    {
                        return PersonMatchingRule.Factory.FromSnapshot(r, parametersSnapshots.Where(p => p.RuleId == r.Id).ToList());
                    }).ToList(),
                };
            }
        }
    }
}
