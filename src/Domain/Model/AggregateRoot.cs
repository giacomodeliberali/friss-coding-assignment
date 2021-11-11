using System;

namespace Domain.Model
{
    /// <summary>
    /// The DDD aggregate root.
    /// </summary>
    public abstract class AggregateRoot
    {
        /// <summary>
        /// The aggregate identifier.
        /// </summary>
        public Guid Id { get; internal set; }
    }
}
