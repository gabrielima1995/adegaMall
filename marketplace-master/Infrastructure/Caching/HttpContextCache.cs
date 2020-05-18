using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Caching;

namespace Infrastructure.Caching
{
    public class HttpContextCache : ICache
    {
        private static TimeSpan _defaultTimeout = new TimeSpan(1, 0, 0);
        private static readonly object _lock = new object();

        private IList<string> _allKeys;
        private readonly Cache _store;

        public HttpContextCache()
        {
            _store = HttpContext.Current?.Cache;

            if (_store == null)
                throw new InvalidOperationException("No current HTTP context available.");

            _allKeys = new List<string>();
        }

        public void Add(string key, object value)
        {
            Add(key, value, _defaultTimeout);
        }

        public void Add(string key, object value, TimeSpan timeout)
        {
            if (Contains(key))
            {
                Remove(key);
            }

            _store.Insert(key, value, null, Cache.NoAbsoluteExpiration, timeout);
            _allKeys.Add(key);
        }

        public void Add(string key, object value, TimeSpan timeout, CacheTimeoutType timeoutType = CacheTimeoutType.Absolute)
        {
            DateTime absoluteExpiration;
            TimeSpan slidingExpiration;

            if (timeoutType == CacheTimeoutType.Absolute)
            {
                absoluteExpiration = DateTime.UtcNow.Add(timeout);
                slidingExpiration = Cache.NoSlidingExpiration;
            }
            else
            {
                absoluteExpiration = Cache.NoAbsoluteExpiration;
                slidingExpiration = timeout;
            }

            _store.Add(
                key, value, null,
                absoluteExpiration, slidingExpiration,
                System.Web.Caching.CacheItemPriority.Default,
                CacheItemRemovedCallback);

            _allKeys.Add(key);

            throw new NotImplementedException();
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

        public void RemovedCallback(CacheEntryRemovedArguments arguments)
        {
            var key = arguments.CacheItem.Key;

            if (_allKeys.Contains(key))
            {
                lock (_lock)
                {
                    if (_allKeys.Contains(key))
                        _allKeys.Remove(key);
                }
            }
        }

        public void CacheItemRemovedCallback(string key, object value, CacheItemRemovedReason reason)
        {
        }
    }
}
