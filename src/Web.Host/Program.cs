using System;
using System.IO;
using System.Threading.Tasks;
using Application.Seed;
using EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Web.Host
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                var hostBuilder = CreateHostBuilder(args).Build();

                // seed database
                // Note: wrong approach here, because if you run the app in multiple instance you end up with concurrency problems or duplicated migrations.
                // We should seed inside migrations applied prior to app start (maybe in a different migrator project).
                using var scope = hostBuilder.Services.CreateScope();
                var applicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                await applicationDbContext!.Database.EnsureCreatedAsync();
                foreach (var dataSeedContributor in scope.ServiceProvider.GetServices<IDataSeedContributor>())
                {
                    await dataSeedContributor.SeedAsync();
                }

                await hostBuilder.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
