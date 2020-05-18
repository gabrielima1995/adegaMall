using System;

namespace Infrastructure.Caching
{
    public class CacheEvictionEventArgs : EventArgs
    {
        public string CacheKey { get; private set; }

        public CacheEvictionEventArgs(string cacheKey)
        {
            CacheKey = cacheKey;
        }
    }
}
