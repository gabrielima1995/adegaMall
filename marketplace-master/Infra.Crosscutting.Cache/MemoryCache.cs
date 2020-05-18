using System;
using System.Linq;
using System.Collections.Generic;

namespace Infra.Crosscutting.Cache
{
    public class MemoryCache : ICache
    {
        private IList<string> _allKeys;

        private System.Runtime.Caching.MemoryCache _store;

        public MemoryCache()
        {
            _allKeys = new List<string>();
            _store = new System.Runtime.Caching.MemoryCache("INFRA_CROSSCUTTING_CACHE_MEMORY");
        }

        public void Add(string key, object value)
        {
            Add(key, value, CacheConfig.DefaultTimeout);
        }

        public void Add(string key, object value, TimeSpan timeout)
        {
            _store.Add(key, value, DateTime.UtcNow.Add(timeout));
            _allKeys.Add(key);
        }

        public bool Contains(string key)
        {
            return _store.Contains(key);
        }

        public void Dispose()
        {
            _store.Dispose();
            _allKeys.Clear();
            _allKeys = null;
        }

        public object Get(string key)
        {
            return _store.Get(key);
        }

        public T Get<T>(string key)
        {
            return (T)Get(key);
        }

        public void Remove(string key)
        {
            if (_store.Contains(key))
            {
                _store.Remove(key);
            }

            _allKeys.Remove(key);
        }

        public void RemoveAll()
        {
            foreach (var key in _allKeys.ToList())
            {
                _store.Remove(key);
            }
        }
    }
}
