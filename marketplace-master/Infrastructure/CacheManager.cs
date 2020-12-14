using Domain.Interfaces;
using Infrastructure.Caching;

namespace Infrastructure
{
    public class CacheManager
    {
        public static CacheManager Instance { get; set; }

        private readonly ICache _cache;

        public CacheManager(ICache cache)
        {
            _cache = cache;
        }

        public void Add<T>(CacheDefinition query, T value)
        {
            if (query.Timeout == null) _cache.Add(query.CacheKey, value);
            else _cache.Add(query.CacheKey, value, query.Timeout.Value, query.TimeoutType);
        }

        public bool Contains(string cacheKey) => _cache.Contains(cacheKey);

        public T Get<T>(string cacheKey)
        {
            return _cache.Get<T>(cacheKey);
        }

        public void Remove(string cacheKey)
        {
             _cache.Remove(cacheKey);
        }
    }
}
