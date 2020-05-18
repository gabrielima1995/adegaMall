using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;

namespace Infrastructure.Logging
{
    internal static class WebExtensions
    {
        public static Dictionary<string, string> ToDictionary(this HttpHeaders headers)
        {
            var dict = new Dictionary<string, string>();

            foreach (var item in headers.ToList())
            {
                if (item.Value != null)
                {
                    var header = item.Value.Aggregate(string.Empty, (current, value) => current + value + " ");

                    // Trim the trailing space and add item to the dictionary
                    header = header.TrimEnd(' ');

                    dict.Add(item.Key.Replace("-", "_"), header);
                }
            }

            return dict;
        }

        public static Dictionary<string, string> ToDictionary(this WebHeaderCollection headers)
        {
            var dict = new Dictionary<string, string>();

            foreach (var key in headers.AllKeys)
            {
                var values = headers.GetValues(key);

                if (values != null)
                {
                    var header = values.Aggregate(string.Empty, (current, value) => current + value + " ");

                    // Trim the trailing space and add item to the dictionary
                    header = header.TrimEnd(' ');

                    dict.Add(key.Replace("-", "_"), header);
                }
            }

            return dict;
        }
    }
}