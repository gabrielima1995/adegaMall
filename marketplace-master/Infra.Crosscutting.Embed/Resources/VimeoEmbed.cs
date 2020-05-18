using System;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Infra.Crosscutting.Embed.Resources
{
    public class VimeoEmbed : ResourceEmbed
    {
        private const string Template = "<iframe src=\"//player.vimeo.com/video/{0}?autoplay={1}\" width=\"500\" height=\"281\" frameborder=\"0\" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>";
        private const string REGEX_VIMEO = @"^.+vimeo.com\/(.*\/)?([^#\?]*)";

        public VimeoEmbed()
            : base(Template)
        {
        }

        public override bool IsResource(string url)
        {
            return url.Contains("vimeo.com");
        }

        public override string CreateEmbed(string url, bool autoPlay = false)
        {
            var match = Regex.Match(url, REGEX_VIMEO);

            if (match.Success)
            {
                return ProcessTemplate(match.Groups[2], autoPlay ? "1" : "0");
            }

            return new UnknownEmbed().CreateEmbed(url);
        }

        public override string GetThumbnail(string url)
        {
            string thumbUrl = EmbedHelper.Config.DefaultVideoThumbnail;

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    Regex regex = new Regex(@"<meta\s*property=""og:image""\s*content=""([^""]*)""");
                    var httpContent = response.Content.ReadAsStringAsync().Result;
                    var match = regex.Match(httpContent);

                    thumbUrl = match.Groups[1].Value;
                }
            }

            return thumbUrl;
        }
    }
}