using Infrastructure.Caching;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    internal static class CacheHelper
    {
        public static T Get<T>(Func<T> query, CacheDefinition cacheDefinition = null)
        {
            if (cacheDefinition != null)
            {
                var cachedResult = CacheManager.Instance.Get<T>(cacheDefinition.CacheKey);
                if (cachedResult != null) return cachedResult;
            }

            var result = query.Invoke();

            if (cacheDefinition != null && result != null)
                CacheManager.Instance.Add(cacheDefinition, result);

            return result;
        }

        public static T Get<T>(IQueryable<T> query, CacheDefinition cacheDefinition = null)
        {
            if (cacheDefinition != null)
            {
                var cachedResult = CacheManager.Instance.Get<T>(cacheDefinition.CacheKey);
                if (cachedResult != null) return cachedResult;
            }

            var result = query.FirstOrDefault();

            if (cacheDefinition != null && result != null)
                CacheManager.Instance.Add(cacheDefinition, result);

            return result;
        }

        public static List<T> List<T>(Func<List<T>> query, CacheDefinition cacheDefinition = null)
        {
            if (cacheDefinition != null)
            {
                var cachedResult = CacheManager.Instance.Get<List<T>>(cacheDefinition.CacheKey);
                if (cachedResult != null) return cachedResult;
            }

            var result = query.Invoke();

            if (cacheDefinition != null && result.Count > 0)
                CacheManager.Instance.Add(cacheDefinition, result);

            return result;
        }

        public static List<T> List<T>(IQueryable<T> query, CacheDefinition cacheDefinition = null)
        {
            if (cacheDefinition != null)
            {
                var cachedResult = CacheManager.Instance.Get<List<T>>(cacheDefinition.CacheKey);
                if (cachedResult != null) return cachedResult;
            }

            var result = query.ToList();

            if (cacheDefinition != null && result.Count > 0)
                CacheManager.Instance.Add(cacheDefinition, result);

            return result;
        }

        public static event EventHandler<CacheEvictionEventArgs> Evicted;

        public static void Evict(string cacheKey)
        {
            CacheManager.Instance.Remove(cacheKey);
            Evicted?.Invoke(null, new CacheEvictionEventArgs(cacheKey));
        }
    }
}
