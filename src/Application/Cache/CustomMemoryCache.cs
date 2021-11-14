using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Cache
{
    /// <summary>
    /// Wraps an <see cref="IMemoryCache"/> to expose the list of keys.
    /// </summary>
    public class CustomMemoryCache : ICustomMemoryCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CustomMemoryCache> _logger;
        private readonly ConcurrentDictionary<object, object> _keys;

        /// <summary>
        /// Initialize a new instance of <see cref="CustomMemoryCache"/>.
        /// </summary>
        public CustomMemoryCache(
            IMemoryCache memoryCache,
            ILogger<CustomMemoryCache> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _keys = new ConcurrentDictionary<object, object>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _memoryCache.Dispose();
        }

        /// <inheritdoc />
        public bool TryGetValue(object key, out object value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }

        /// <inheritdoc />
        public ICacheEntry CreateEntry(object key)
        {
            _logger.LogDebug("Creating entry {Key}", key);

            var entry = _memoryCache.CreateEntry(key);

            _keys.TryAdd(key, null);

            entry.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration()
            {
                EvictionCallback = (evictedKey, value, reason, state) =>
                {
                    _logger.LogDebug("Removing evicted entry {Key}", evictedKey);
                    _keys.TryRemove(evictedKey, out _);
                }
            });

            return entry;
        }

        /// <inheritdoc />
        public void Remove(object key)
        {
            _memoryCache.Remove(key);
            _keys.TryRemove(key, out _);
            _logger.LogDebug("Removed entry {Key}", key);
        }

        /// <inheritdoc />
        public void RemoveIf(Func<object, bool> condition)
        {
            foreach (var (key, _) in _keys)
            {
                if (condition(key))
                {
                    Remove(key);
                }
            }
        }
    }
}
