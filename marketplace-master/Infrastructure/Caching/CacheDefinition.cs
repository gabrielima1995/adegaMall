using Domain.Interfaces;
using System;

namespace Infrastructure.Caching
{
    public class CacheDefinition
    {
        public string CacheKey { get; set; }
        public TimeSpan? Timeout { get; set; }
        public CacheTimeoutType TimeoutType { get; set; }

        public CacheDefinition(string cacheKey, TimeSpan? timeout = null, CacheTimeoutType timeoutType = CacheTimeoutType.Absolute)
        {
            if (string.IsNullOrWhiteSpace(cacheKey)) throw new ArgumentNullException(nameof(cacheKey));
            CacheKey = cacheKey;
            Timeout = timeout;
            TimeoutType = timeoutType;
        }
    }
}
