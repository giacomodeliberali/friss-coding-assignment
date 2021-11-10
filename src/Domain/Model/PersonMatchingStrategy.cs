using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;

namespace Domain.Model
{
    public class PersonMatchingStrategy : Entity
    {
        private List<PersonMatchingRule> _rules;

        public string Name { get; protected set; }

        public string Description { get; protected set; }

        public IReadOnlyCollection<PersonMatchingRule> Rules => _rules;

        private PersonMatchingStrategy()
        {
        }

        public async Task<decimal> Match(Person first, Person second)
        {
            PersonMatchingRuleDelegate finalNext = (finalScore) =>
            {
                // _logger.Debug($"Pipeline pre-processors terminata con pre-processor di default per Questionnaire '{questionnaire.Resource.Name}'");
                return Task.FromResult(finalScore);
            };

            var rulesPipelines = Rules
                .Reverse()
                .Aggregate(
                    finalNext,
                    (next, currentRule) =>
                    {
                        return async (score) =>
                        {
                            if (currentRule.IsEnabled)
                            {
                                return await currentRule.Match(first, second, score, next);
                            }

                            return score;
                        };
                    });

            return await rulesPipelines(0);
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
                Guid id,
                string name,
                string description,
                List<PersonMatchingRule> rules)
            {
                return new PersonMatchingStrategy()
                {
                    Id = id,
                    Name = name,
                    Description = description,
                    _rules = rules,
                };
            }
        }
    }
}
