using Application.Modules.BatchImport;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace Infra.BatchImportAdapter
{
    public class FacebookImageProvider : ISocialImageProvider
    {
        private const string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.67 Safari/537.36";

        private static readonly Regex imgTag = new Regex(@"<img[^\>]class=._11kf\simg[^\>]+\/>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex srcAttribute = new Regex(@"src=.([^""]+).", RegexOptions.Compiled);

        public bool CanHandleUrl(string profileUrl)
        {
            return
                profileUrl.StartsWith("https://fb.com/", StringComparison.OrdinalIgnoreCase) ||
                profileUrl.StartsWith("https://www.fb.com/", StringComparison.OrdinalIgnoreCase) ||
                profileUrl.StartsWith("https://facebook.com/", StringComparison.OrdinalIgnoreCase) ||
                profileUrl.StartsWith("https://www.facebook.com/", StringComparison.OrdinalIgnoreCase);
        }

        public byte[] GetImage(string profileUrl)
        {
            var html = DownloadPageHtml(profileUrl);

            if (html == null)
                return null;

            var pictureUrl = ExtractPictureUrl(html);

            if (pictureUrl == null)
                return null;

            return DownloadImage(pictureUrl);
        }

        private byte[] DownloadImage(string pictureUrl)
        {
            try
            {
                using (var client = new WebClient())
                    return client.DownloadData(pictureUrl);
            }
            catch { return null; }
        }

        private string DownloadPageHtml(string profileUrl)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("user-agent", userAgent);
                    return client.DownloadString(profileUrl);
                }
            }
            catch { return null; }
        }

        private string ExtractPictureUrl(string html)
        {
            if (!imgTag.IsMatch(html))
                return null;

            var tag = imgTag.Match(html).Value;
            var url = srcAttribute.Match(tag).Groups[1].Value;

            return HttpUtility.HtmlDecode(url);
        }
    }
}
