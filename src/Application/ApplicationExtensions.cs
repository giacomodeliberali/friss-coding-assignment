using System;
using System.Linq;
using Application.Rules;
using Application.Services;
using Domain;
using Domain.Rules;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    /// <summary>
    /// Extensions to register application services into the DI container.
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Registers all the application services into the DI container.
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void AddApplicationServices(this IServiceCollection serviceCollection)
        {
            // This registration could be made automatically by combining assembly scanning and convention on the class names
            serviceCollection.AddTransient<IPersonApplicationService, PersonApplicationService>();
            serviceCollection.AddTransient<IStrategyMatchApplicationService, StrategyMatchApplicationService>();
            serviceCollection.AddTransient<IMatchingRuleStrategyExecutor, MatchingRuleStrategyExecutor>();

           serviceCollection.AddRuleTypes();
        }

        private static void AddRuleTypes(this IServiceCollection serviceCollection)
        {
            var ruleContributorType = typeof(IRuleContributor);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .AsParallel()
                .SelectMany(x => x.GetTypes())
                .Where(x => !x.IsInterface && !x.IsAbstract)
                .Where(x => ruleContributorType.IsAssignableFrom(x));

            foreach (var type in types)
            {
                serviceCollection.AddTransient(type);
            }
        }
    }
}
