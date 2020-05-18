using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Infrastructure.Caching
{
    public class MemoryCache : ICache
    {
        private static TimeSpan defaultTimeout = new TimeSpan(1, 0, 0);
        private static readonly object theLock = new object();

        private IList<string> allKeys;
        private readonly System.Runtime.Caching.MemoryCache store;

        public MemoryCache()
        {
            allKeys = new List<string>();
            store = new System.Runtime.Caching.MemoryCache("__Infrastructure_MemoryCache");
        }

        public void Add(string key, object value)
        {
            Add(key, value, defaultTimeout);
        }

        public void Add(string key, object value, TimeSpan timeout, CacheTimeoutType timeoutType = CacheTimeoutType.Absolute)
        {
            if (value == null) return;

            var policy = new CacheItemPolicy { RemovedCallback = RemovedCallback };
            
            if (timeoutType == CacheTimeoutType.Absolute) policy.AbsoluteExpiration = DateTime.UtcNow.Add(timeout);
            else if (timeoutType == CacheTimeoutType.Sliding) policy.SlidingExpiration = timeout;

            store.Add(key, value, policy);
            allKeys.Add(key);
        }

        public bool Contains(string key) => store.Contains(key);

        public void Dispose()
        {
            store.Dispose();
            allKeys.Clear();
            allKeys = null;
        }

        public object Get(string key) => store.Get(key);

        public T Get<T>(string key) => (T)Get(key);

        public void Remove(string key)
        {
            if (store.Contains(key))
                store.Remove(key);

            allKeys.Remove(key);
        }

        public void RemoveAll()
        {
            foreach (var key in allKeys.ToList())
                store.Remove(key);
        }

        public void RemovedCallback(CacheEntryRemovedArguments arguments)
        {
            var key = arguments.CacheItem.Key;

            if (allKeys.Contains(key))
            {
                lock (theLock)
                {
                    if (allKeys.Contains(key))
                        allKeys.Remove(key);
                }
            }
        }
    }
}
