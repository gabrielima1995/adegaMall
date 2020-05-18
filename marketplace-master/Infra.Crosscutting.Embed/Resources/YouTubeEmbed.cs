using System;
using System.Text.RegularExpressions;

namespace Infra.Crosscutting.Embed.Resources
{
    public class YouTubeEmbed : ResourceEmbed
    {
        private const string Template = "<iframe width=\"560\" height=\"315\" src=\"//www.youtube.com/embed/{0}?rel=0&autoplay={1}\" frameborder=\"0\" allowfullscreen></iframe>";
        private const string REGEX_YOUTUBE = @".*(youtu.be\/|v\/\w+\/|embed\/|v=)([^#\&\?]*).*";

        public YouTubeEmbed()
            : base(Template)
        {
        }

        public override bool IsResource(string url)
        {
            return url.Contains("youtube.com") || url.Contains("youtu.be");
        }

        public override string CreateEmbed(string url, bool autoPlay = false)
        {
            var match = Regex.Match(url, REGEX_YOUTUBE);

            if (match.Success)
            {
                return ProcessTemplate(match.Groups[2], autoPlay ? "1" : "0");
            }

            return new UnknownEmbed().CreateEmbed(url);
        }

        public override string GetThumbnail(string url)
        {
            var match = Regex.Match(url, REGEX_YOUTUBE);

            if (match.Success)
            {
                return string.Format("http://img.youtube.com/vi/{0}/hqdefault.jpg", match.Groups[2]);
            }

            return EmbedHelper.Config.DefaultVideoThumbnail;
        }
    }
}