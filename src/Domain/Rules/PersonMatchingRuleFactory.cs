using System;
using Domain.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Rules
{
    public class PersonMatchingRuleFactory : IPersonMatchingRuleFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PersonMatchingRuleFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public PersonMatchingRule GetInstance(string assemblyQualifiedName, string name, string description, Guid? id)
        {
            var ruleType = Type.GetType(assemblyQualifiedName);

            // @GDL handle exceptions and validation

            var instance = (PersonMatchingRule)ActivatorUtilities.CreateInstance(
                _serviceProvider,
                ruleType);

            instance.Name = name;
            instance.Description = description;
            instance.Id = id ?? Guid.NewGuid();

            return instance;
        }
    }
}
