using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Model
{
    public delegate Task<decimal> PersonMatchingRuleDelegate(decimal score);

    public abstract class PersonMatchingRule : Entity
    {
        private List<MatchingRuleParameter> _parameters;
        public string Name { get; internal set; }

        public string Description { get; internal set; }

        public bool IsEnabled { get; protected set; }

        public IReadOnlyCollection<MatchingRuleParameter> Parameters => _parameters;

        internal PersonMatchingRule()
        {
            Id = Guid.NewGuid();
            IsEnabled = true;
            _parameters = new List<MatchingRuleParameter>();
        }

        public void AddParameter(MatchingRuleParameter parameter)
        {
            _parameters.Add(parameter);
        }

        protected decimal GetParameterOrDefault(string name, decimal defaultValue)
        {
            var parameter = Parameters.SingleOrDefault(p => p.Name == name);

            if (parameter is not null)
            {
                return parameter.Value;
            }

            return defaultValue;
        }

        public abstract Task<decimal> Match(Person first, Person second, decimal score, PersonMatchingRuleDelegate next);
    }
}
