using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Helpers
{
    public static class ImgHelper
    {
        private static bool configured;
        private static string imageResizerBaseUrl;
        private static string storageBaseUrl;
        private static string cdnEndpoint;
        private static string defaultUserAvatar;
        private static string defaultProductImage;

        public static void Configure(string storageBaseUrl, string imageResizerBaseUrl, string cdnEndpoint, string defaultUserAvatar, string defaultProductImage)
        {
            ImgHelper.imageResizerBaseUrl = SanitizeUrl(imageResizerBaseUrl);
            ImgHelper.storageBaseUrl = SanitizeUrl(storageBaseUrl);
            ImgHelper.cdnEndpoint = SanitizeUrl(cdnEndpoint);
            ImgHelper.defaultUserAvatar = defaultUserAvatar;
            ImgHelper.defaultProductImage = defaultProductImage;
            configured = true;
        }

        public static string Avatar(string imageUrl, int size)
        {
            return Get(imageUrl, size, size, defaultUserAvatar);
        }

        public static string Product(string imageUrl, int size)
        {
            return Get(imageUrl, size, size, defaultProductImage);
        }

        public static string Get(string imageUrl, int width, int height, string defaultUrl = null, string mode = "crop")
        {
            if (imageUrl == null) return defaultUrl;
            return BlobToCdn(imageUrl, new { w = width, h = height, mode = mode, scale = "both" });
        }

        public static string BlobToCdn(string imageUrl, object param)
        {
            if (!configured) throw new Exception("ImgHelper has not been configured. Call ImgHelper.Configure(...) before use.");
            if (string.IsNullOrEmpty(imageUrl)) return imageUrl;

            var newUrl = imageUrl.Replace(storageBaseUrl, imageResizerBaseUrl);

            if (cdnEndpoint != null)
            {
                var cdnUrl = cdnEndpoint;

                if (newUrl.StartsWith("/"))
                    newUrl = cdnUrl + newUrl;

                else newUrl = BuildUrl(cdnUrl, newUrl).ToString();
            }

            if (param != null)
            {
                var qs = param.ToDictionary().ToQueryString();
                newUrl += (newUrl.Contains("?") ? "&" : "?") + qs;
            }
            
            return newUrl;
        }

        private static string SanitizeUrl(string url)
        {
            if (url == null) return null;
            if (!url.EndsWith("/")) url += "/";
            return url;
        }

        private static string BuildUrl(string baseUrl, string relativeUrl)
        {
            return new Uri(new Uri(baseUrl), new Uri(relativeUrl).PathAndQuery).ToString();
        }
    }
}