using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace NewContext.Infrastructure.Util
{
    internal interface ICachingProvider
    {
        TItem Get<TItem>(string key, int slidingExpirationInMinutes, Func<TItem> factory);

        Task<TItem> GetAsync<TItem>(string key, int slidingExpirationInMinutes, Func<Task<TItem>> factory);

        void Remove(string key);
    }

    internal class MemoryCachingProvider : ICachingProvider
    {
        private readonly IMemoryCache cache;

        public MemoryCachingProvider(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public TItem Get<TItem>(string key, int slidingExpirationInMinutes, Func<TItem> factory)
        {
            if (!cache.TryGetValue(key, out TItem cachedValue))
            {
                cachedValue = factory();
                cache.Set(key, cachedValue, GetOptions(slidingExpirationInMinutes));
            }

            return cachedValue;
        }

        public async Task<TItem> GetAsync<TItem>(string key, int slidingExpirationInMinutes, Func<Task<TItem>> factory)
        {
            if (!cache.TryGetValue(key, out TItem cachedValue))
            {
                cachedValue = await factory();
                cache.Set(key, cachedValue, GetOptions(slidingExpirationInMinutes));
            }

            return cachedValue;
        }

        public void Remove(string key)
        {
            cache.Remove(key);
        }

        private MemoryCacheEntryOptions GetOptions(int slidingExpirationInMinutes)
        {
            return new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(slidingExpirationInMinutes));
        }
    }
}
