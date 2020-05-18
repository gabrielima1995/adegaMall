using System;

namespace Infra.Crosscutting.Cache
{
    public static class CacheConfig
    {
        static CacheConfig()
        {
            DefaultTimeout = new TimeSpan(1, 0, 0);
        }

        public static TimeSpan DefaultTimeout { get; set; }
    }
}
