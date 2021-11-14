using System.Threading.Tasks;

namespace Domain.Repositories
{
    /// <summary>
    /// A base interface for repository to simulate a UnitOfWork.
    /// </summary>
    public interface IBaseRepository
    {
        /// <summary>
        /// Commits the active changes.
        /// </summary>
        Task SaveChangesAsync();
    }
}
