using System;
using Domain.Model;

namespace Domain.Rules
{
    public interface IPersonMatchingRuleFactory
    {
        PersonMatchingRule GetInstance(
            string assemblyQualifiedName,
            string name,
            string description,
            Guid? id = null);
    }
}
