using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;

namespace Infra.Crosscutting.Cache
{
    public class HttpContextCache : ICache
    {
        private IList<string> _allKeys;

        private System.Web.Caching.Cache _store;

        public HttpContextCache()
        {
            _store = HttpContext.Current?.Cache;

            if (_store == null) throw new InvalidOperationException("No current HTTP context available.");

            _allKeys = new List<string>();
        }

        public void Add(string key, object value)
        {
            Add(key, value, CacheConfig.DefaultTimeout);
        }

        public void Add(string key, object value, TimeSpan timeout)
        {
            if (Contains(key))
            {
                Remove(key);
            }

            _store.Insert(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, timeout);
            _allKeys.Add(key);
        }

        public bool Contains(string key)
        {
            return _store.Get(key) != null;
        }

        public void Dispose()
        {
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
            if (Contains(key))
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
