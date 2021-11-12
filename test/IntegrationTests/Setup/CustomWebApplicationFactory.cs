using System.Data.Common;
using Application.Seed;
using EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IntegrationTests.Setup
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var connection = CreateInMemoryDatabase();

            builder.ConfigureServices(services =>
            {
                // remove EF on SqlLite
                services.RemoveAll<ApplicationDbContext>();
                services.RemoveAll<DbContextOptions<ApplicationDbContext>>();

                // Add a database context (ApplicationDbContext) using an in-memory database for testing.
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlite(connection);
                });

                var serviceProvider = services.BuildServiceProvider();

                var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                dbContext.Database.EnsureCreated();

                foreach (var dataSeedContributor in scope.ServiceProvider.GetServices<IDataSeedContributor>())
                {
                    dataSeedContributor.SeedAsync();
                }
            });
        }
    }
}
