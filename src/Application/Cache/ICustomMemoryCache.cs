using System;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Cache
{
    /// <summary>
    /// Represents a local in-memory cache whose values are not serialized.
    /// </summary>
    public interface ICustomMemoryCache : IMemoryCache
    {
        /// <summary>
        /// Removes the keys that match the condition.
        /// </summary>
        /// <param name="condition">Removes the key if this returns true.</param>
        public void RemoveIf(Func<object, bool> condition);
    }
}
