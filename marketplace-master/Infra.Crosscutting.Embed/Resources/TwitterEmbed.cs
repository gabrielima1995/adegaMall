using System;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Infra.Crosscutting.Embed.Resources
{
    public class TwitterEmbed : ResourceEmbed
    {
        private const string Template = "<blockquote class=\"twitter-tweet\" lang=\"pt-BR\"><a href=\"{0}\"/></blockquote>\n<script async src=\"//platform.twitter.com/widgets.js\" charset=\"utf-8\"></script>";

        public TwitterEmbed()
            : base(Template)
        {
        }

        public override bool IsResource(string url)
        {
            return url.Contains("twitter.com");
        }

        public override string CreateEmbed(string url, bool autoPlay = false)
        {
            return ProcessTemplate(url);
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

            //return "/assets/images/logo-twitter.png";
        }
    }
}