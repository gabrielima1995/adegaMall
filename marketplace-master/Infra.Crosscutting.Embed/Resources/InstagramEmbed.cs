using System;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Infra.Crosscutting.Embed.Resources
{
    public class InstagramEmbed : ResourceEmbed
    {
        private const string Template = "<iframe src=\"//instagram.com/p/{0}/embed/\" width=\"612\" height=\"710\" frameborder=\"0\" scrolling=\"no\" allowtransparency=\"true\"></iframe>";
        private const string REGEX_INSTAGRAM = @"\/p\/(.*)/";

        public InstagramEmbed()
            : base(Template)
        {
        }

        public override bool IsResource(string url)
        {
            return url.Contains("instagram.com");
        }

        public override string CreateEmbed(string url, bool autoPlay = false)
        {
            var match = Regex.Match(url, REGEX_INSTAGRAM);

            if (match.Success)
            {
                return ProcessTemplate(match.Groups[1]);
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