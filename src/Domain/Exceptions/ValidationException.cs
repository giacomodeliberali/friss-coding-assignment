using System;

namespace Domain.Exceptions
{
    /// <summary>
    /// Base class for all validation exception.
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="message">The message.</param>
        public ValidationException(string message)
        : base(message)
        {
        }
    }
}
