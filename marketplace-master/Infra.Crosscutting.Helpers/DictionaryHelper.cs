using System.Linq;

namespace System.Collections.Generic
{
    public static class DictionaryHelper
    {
        public static string ToQueryString(this IDictionary<string, string> dict)
        {
            var aux = dict
                .Select(i => $"{i.Key}={i.Value}");

            return string.Join("&", aux);
        }
    }
}