using System.Collections.Generic;
using System.Linq;

namespace System.Dynamic
{
    public static class DynamicExtensions
    {
        public static dynamic DynamicFrom(Dictionary<string, object> propertiesAndValues)
        {
            dynamic d = propertiesAndValues.Aggregate(
                new ExpandoObject() as IDictionary<string, object>,
                (a, p) => { a.Add(p.Key, p.Value); return a; });

            return d;
        }
    }
}
