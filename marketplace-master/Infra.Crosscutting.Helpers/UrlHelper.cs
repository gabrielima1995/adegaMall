using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System
{
    public static class UrlHelper
    {
        public static string DisplayUrl(this string url)
        {
            if (string.IsNullOrEmpty(url)) return "";

            return url
                .Replace("http://www.", "")
                .Replace("https://www.", "")
                .Replace("http://", "")
                .Replace("https://", "");
        }

        public static Uri Append(this Uri uri, params string[] paths)
        {
            return new Uri(paths.Aggregate(uri.AbsoluteUri, (current, path) => string.Format("{0}/{1}", current.TrimEnd('/'), path.TrimStart('/'))));
        }

        public static string GetFileName(this Uri uri)
        {
            for (var i = uri.Segments.Length - 1; i >= 0; i--)
            {
                var item = uri.Segments[i].Replace("/", "");

                if (item.Length > 3)
                {
                    var ext = Path.GetExtension(item);

                    if (!string.IsNullOrEmpty(ext))
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        public static bool IsUrl(this string url)
        {
            var regex = new Regex(@"(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-‌​\.\?\,\'\/\\\+&amp;%\$!#\=_]*)?$", RegexOptions.IgnoreCase);
            return regex.IsMatch(url);
        }

        public static bool IsShortUrl(this string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            // 1. Overall URL length - May be a max of 30 charecters
            if (url.Length > 30) return false;

            var uri = new Uri(url);

            // No query string & no fragment
            if (!string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment)) return false;

            var pathParts = uri.AbsolutePath.Split('/');

            // 3. Number of '/' after protocol (http://) - Max 2
            if (pathParts.Length > 2) return false;

            // 2. URL length after last '/' - May be a max of 10 characters
            var lastOrDefault = pathParts.LastOrDefault();
            if (lastOrDefault != null && lastOrDefault.Length > 10) return false;

            // 4. Max length of host
            if (uri.Host.Length > 13) return false;

            return true;
        }

        public static async Task<string> ResolveShortUrlAsync(this string url)
        {
            if (string.IsNullOrEmpty(url)) return url;

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";
            request.UserAgent = "inabot";
            request.AllowAutoRedirect = false;

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
                if (response.StatusCode == HttpStatusCode.MovedPermanently || response.StatusCode == HttpStatusCode.Found)
                {
                    var location = response.Headers["Location"];

                    if (!string.IsNullOrEmpty(location))
                    {
                        // verifica se o location retornado é uma url encurtada também
                        if (location != url && location.IsShortUrl())
                            return await location.ResolveShortUrlAsync();

                        return location;
                    }
                }
            }

            return url;
        }

        public static bool IsImageUrl(this string url)
        {
            var fileName = new Uri(url).GetFileName();

            return fileName.IsImageFile();
        }

        public static bool IsVideoUrl(this string url)
        {
            var fileName = new Uri(url).GetFileName();

            return fileName.IsVideoFile();
        }
    }
}