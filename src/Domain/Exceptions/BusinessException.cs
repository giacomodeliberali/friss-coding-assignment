using System;

namespace Domain.Exceptions
{
    /// <summary>
    /// Base class for all domain exceptions.
    /// </summary>
    public abstract class BusinessException : Exception
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="message">The message.</param>
        public BusinessException(string message)
        : base(message)
        {
        }
    }
}
