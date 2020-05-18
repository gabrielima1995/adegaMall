using System;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Infra.Crosscutting.Embed.Resources
{
    public class DailymotionEmbed : ResourceEmbed
    {
        private const string Template = "<iframe frameborder=\"0\" width=\"480\" height=\"270\" src=\"//www.dailymotion.com/embed/video/{0}?autoplay={1}\" allowfullscreen></iframe>";
        private const string REGEX_DAILYMOTION = @"^.+dailymotion.com\/((video|hub)\/([^_]+))?[^#]*(#video=([^_&]+))?";
        private const string REGEX_DAI_LY = @"dai\.ly\/(.*)";

        public DailymotionEmbed()
            : base(Template)
        {
        }

        public override bool IsResource(string url)
        {
            return url.Contains("dailymotion.com") || url.Contains("dai.ly");
        }

        public override string CreateEmbed(string url, bool autoPlay = false)
        {
            var match = Regex.Match(url, REGEX_DAILYMOTION);

            if (match.Success)
            {
                return ProcessTemplate(match.Groups[3], autoPlay ? "1" : "0");
            }

            match = Regex.Match(url, REGEX_DAI_LY);

            if (match.Success)
            {
                return ProcessTemplate(match.Groups[1], autoPlay ? "1" : "0");
            }

            return new UnknownEmbed().CreateEmbed(url);
        }

        public override string GetThumbnail(string url)
        {
            string thumbUrl = EmbedHelper.Config.DefaultVideoThumbnail;
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url.Replace("/video/", "/thumbnail/video/")).Result;
                if (response.IsSuccessStatusCode)
                    thumbUrl = response.RequestMessage.RequestUri.AbsoluteUri;
            }

            return thumbUrl;

            //return url.Replace("/video/", "/thumbnail/video/");
        }
    }
}