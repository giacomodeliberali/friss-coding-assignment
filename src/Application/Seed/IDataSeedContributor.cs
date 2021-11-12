using System.Threading.Tasks;

namespace Application.Seed
{
    /// <summary>
    /// Every class that has to seed some database data must implement this interface.
    /// <remarks>
    /// Wrong approach here, because if you run the app in multiple instance you end up with concurrency problems or duplicated migrations.
    /// We should seed inside migrations applied prior to app start.
    /// </remarks>
    /// </summary>
    public interface IDataSeedContributor
    {
        /// <summary>
        /// Seeds some data into the database.
        /// </summary>
        Task SeedAsync();
    }
}
