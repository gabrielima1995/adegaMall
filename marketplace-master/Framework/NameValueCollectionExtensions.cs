using System.Globalization;
using System.Text;
using System.Threading;
using System.Web;

namespace System.Collections.Specialized
{
    public static class NameValueCollectionExtensions
    {
        public static void Add(this NameValueCollection collection, string name, object value)
        {
            collection.Add(name, ConvertToString(value));
        }

        public static string ToQueryString(this NameValueCollection collection)
        {
            var builder = new StringBuilder();

            for (var i = 0; i < collection.Count; i++)
            {
                var name = collection.AllKeys[i];
                var encodedValue = HttpUtility.UrlEncode(collection[i]);

                if (i > 0)
                    builder.Append("&");

                builder.Append($"{name}={encodedValue}");
            }

            return builder.ToString();
        }

        private static string ConvertToString(object value)
        {
            if (value == null) return null;

            var c = Thread.CurrentThread.CurrentCulture;
            var uic = Thread.CurrentThread.CurrentUICulture;

            try
            {
                Thread.CurrentThread.CurrentCulture =
                    Thread.CurrentThread.CurrentUICulture =
                        CultureInfo.InvariantCulture;

                return value.ToString();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = c;
                Thread.CurrentThread.CurrentUICulture = uic;
            }
        }
    }
}
