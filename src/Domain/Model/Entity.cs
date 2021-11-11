using System;

namespace Domain.Model
{
    /// <summary>
    /// The DDD entity.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// The entity identifier.
        /// </summary>
        public Guid Id { get; internal set; }
    }
}
