using Domain.Exceptions;

namespace Application.Exceptions
{
    /// <summary>
    /// Thrown when attempting to create a strategy with d duplicate name.
    /// </summary>
    public class StrategyAlreadyExistsException : BusinessException
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="name">The strategy duplicated name.</param>
        public StrategyAlreadyExistsException(string name)
            : base($"Strategy with name '{name}' already exists.")
        {
        }
    }
}
