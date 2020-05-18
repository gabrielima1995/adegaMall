using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Framework
{
    public static class SimpleMapper
    {
        private static readonly object _lock = new object();
        private static readonly Dictionary<int, object> _mappingCache = new Dictionary<int, object>();

        public static TDest Map<TSource, TDest>(TSource source) where TSource : class where TDest : class, new()
        {
            var dest = Activator.CreateInstance<TDest>();
            Map(source, dest);
            return dest;
        }

        public static void Map<TSource, TDest>(TSource source, TDest dest) where TSource : class where TDest : class
        {
            var mapping = GetMapping<TSource, TDest>();

            foreach (var m in mapping.Properties)
            {
                var fromProp = m.Item1;
                var toProp = m.Item2;

                var fromValue = fromProp.GetValue(source, null);
                toProp.SetValue(dest, fromValue, null);
            }
        }

        private static Mapping<TSource, TDest> GetMapping<TSource, TDest>()
        {
            var key = (typeof(TSource).FullName + typeof(TDest).FullName).GetHashCode();

            if (!_mappingCache.ContainsKey(key))
            {
                lock (_lock)
                {
                    var mapping = CreateMapping<TSource, TDest>();
                    _mappingCache.Add(key, mapping);
                }
            }

            return _mappingCache[key] as Mapping<TSource, TDest>;
        }

        private static Mapping<TSource, TDest> CreateMapping<TSource, TDest>()
        {
            var m = new Mapping<TSource, TDest>();

            var bindingFlags = BindingFlags.Public | BindingFlags.Instance;
            var fromProps = typeof(TSource).GetProperties(bindingFlags).Where(x => x.CanRead).ToArray();
            var toProps = typeof(TDest).GetProperties(bindingFlags).Where(x => x.CanWrite).ToArray();

            for (var f = 0; f < fromProps.Length; f++)
            {
                for (var t = 0; t < toProps.Length; t++)
                {
                    var fp = fromProps[f];
                    var tp = toProps[t];

                    if (fp.Name == tp.Name && fp.PropertyType == tp.PropertyType)
                    {
                        m.Add(fp, tp);
                    }
                }
            }

            return m;
        }

        private class Mapping<TSource, TDest>
        {
            public List<Tuple<PropertyInfo, PropertyInfo>> Properties { get; set; }

            public Mapping()
            {
                Properties = new List<Tuple<PropertyInfo, PropertyInfo>>();
            }

            public void Add(PropertyInfo from, PropertyInfo to)
            {
                Properties.Add(new Tuple<PropertyInfo, PropertyInfo>(from, to));
            }
        }
    }
}
