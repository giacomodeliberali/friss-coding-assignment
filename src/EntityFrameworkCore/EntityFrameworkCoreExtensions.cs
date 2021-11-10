using Domain.Repositories;
using EntityFrameworkCore.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore
{
    public static class EntityFrameworkCoreExtensions
    {
        public static void AddRepositories(this IServiceCollection serviceCollection)
        {
            // This registrations could be made automatically by combining assembly scanning and convention on the class names
            serviceCollection.AddTransient<IPersonRepository, PersonRepository>();
            serviceCollection.AddTransient<IMatchingStrategyRepository, MatchingStrategyRepository>();
        }
    }
}
