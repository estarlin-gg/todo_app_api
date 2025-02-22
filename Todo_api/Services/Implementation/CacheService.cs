using System.Collections.Concurrent;

namespace Todo_api.Services.Implementation
{
    public class CacheService
    {
        private readonly ConcurrentDictionary<string, object> _cache = new();
        public T GetOrAdd<T>(string key, Func<T> computeValue)
        {
            if (!_cache.TryGetValue(key, out var value))
            {
                value = computeValue();
                _cache[key] = value;
            }
            return (T)value;
        }
    }
}
