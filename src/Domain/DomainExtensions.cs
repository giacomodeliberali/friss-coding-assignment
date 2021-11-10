using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Model;
using Domain.Rules;
using Microsoft.Extensions.DependencyInjection;

namespace Domain
{
    public static class DomainExtensions
    {

        public static void AddDomain(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IPersonMatchingRuleFactory, PersonMatchingRuleFactory>();
        }
    }
}
