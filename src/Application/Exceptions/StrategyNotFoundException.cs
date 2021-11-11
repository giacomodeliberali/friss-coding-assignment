using System;
using Domain.Exceptions;
using Domain.Model;

namespace Application.Exceptions
{
    /// <summary>
    /// Thrown when attempting to run a workflow that requires a non-existing <see cref="MatchingStrategy"/>.
    /// </summary>
    public class StrategyNotFoundException : BusinessException
    {
        /// <summary>
        /// Create the exception for a non-existing strategy name.
        /// </summary>
        /// <param name="name">The strategy name.</param>
        public StrategyNotFoundException(string name)
        : base($"Strategy {name} not found.")
        {
        }

        /// <summary>
        /// Create the exception for a non-existing strategy id.
        /// </summary>
        /// <param name="id">The strategy id.</param>
        public StrategyNotFoundException(Guid id)
            : base($"Strategy {id.ToString()} not found.")
        {
        }
    }
}
