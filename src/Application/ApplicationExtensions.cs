using System;
using System.Linq;
using Application.Cache;
using Application.Rules;
using Application.Seed;
using Application.Services;
using Domain.Rules;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

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

            // Add cache
            serviceCollection.AddSingleton<ICustomMemoryCache, CustomMemoryCache>();

           serviceCollection.AddRuleTypes();
           serviceCollection.AddDataSeedContributors();
        }

        private static void AddRuleTypes(this IServiceCollection serviceCollection)
        {
            var ruleContributorType = typeof(IMatchingRuleContributor);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => !x.IsInterface && !x.IsAbstract)
                .Where(x => ruleContributorType.IsAssignableFrom(x));

            foreach (var type in types)
            {
                Log.ForContext<IMatchingRuleContributor>().Debug("Registering {Type} in DI container", type.FullName);
                serviceCollection.AddTransient(type);
            }
        }

        private static void AddDataSeedContributors(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IDataSeedContributor, DefaultStrategySeed>();
            serviceCollection.AddTransient<IDataSeedContributor, DemoUsersSeed>();
        }
    }
}
