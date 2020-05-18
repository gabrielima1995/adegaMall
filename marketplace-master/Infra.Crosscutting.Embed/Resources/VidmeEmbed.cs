using System;
using System.Text.RegularExpressions;

namespace Infra.Crosscutting.Embed.Resources
{
    public class VidmeEmbed : ResourceEmbed
    {
        private const string Template = "<iframe width=\"320\" height=\"240\" src=\"https://vid.me/e/{0}?autoplay={1}\" frameborder=\"0\" scrolling=\"no\" allowfullscreen webkitallowfullscreen mozallowfullscreen></iframe>";
        private const string REGEX_VIDME = @"vid\.me\/(.*)";

        public VidmeEmbed()
            : base(Template)
        {
        }

        public override bool IsResource(string url)
        {
            return url.Contains("vid.me");
        }

        public override string CreateEmbed(string url, bool autoPlay = false)
        {
            var match = Regex.Match(url, REGEX_VIDME);

            if (match.Success)
            {
                return ProcessTemplate(match.Groups[1], autoPlay ? "1" : "0");
            }

            return new UnknownEmbed().CreateEmbed(url);
        }

        public override string GetThumbnail(string url)
        {
            return EmbedHelper.Config.DefaultVideoThumbnail;
        }
    }
}