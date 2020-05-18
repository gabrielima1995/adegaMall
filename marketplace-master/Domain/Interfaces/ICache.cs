using System;
using System.Runtime.Caching;

namespace Domain.Interfaces
{
    public interface ICache
    {
        void Add(string key, object value);

        void Add(string key, object value, TimeSpan timeout, CacheTimeoutType timeoutType);

        bool Contains(string key);

        object Get(string key);

        T Get<T>(string key);

        void Remove(string key);

        void RemoveAll();

        void RemovedCallback(CacheEntryRemovedArguments arguments);
    }

    public enum CacheTimeoutType
    {
        Absolute = 0,
        Sliding = 1
    }
}
