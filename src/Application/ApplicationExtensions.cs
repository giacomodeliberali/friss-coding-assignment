using Application.Services;
using Domain;
using Domain.Rules;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationExtensions
    {
        public static void AddApplicationServices(this IServiceCollection serviceCollection)
        {
            // This registration could be made automatically by combining assembly scanning and convention on the class names
            serviceCollection.AddTransient<IPersonApplicationService, PersonApplicationService>();
            serviceCollection.AddTransient<IPersonStrategyMatchApplicationService, PersonStrategyMatchApplicationService>();

            // @GDL should it be used in startup or here?
            serviceCollection.AddDomain();
        }
    }
}
