using System;
using System.Text.RegularExpressions;

namespace Infra.Crosscutting.Embed.Resources
{
    public class VineEmbed : ResourceEmbed
    {
        private const string Template = "<iframe class=\"vine-embed\" src=\"https://vine.co/v/{0}/embed/postcard\" width=\"600\" height=\"600\" frameborder=\"0\"></iframe><script async src=\"//platform.vine.co/static/scripts/embed.js\" charset=\"utf-8\"></script>";
        private const string REGEX_VINE = @"vine\.co\/v\/(\w*)";

        public VineEmbed()
            : base(Template)
        {
        }

        public override bool IsResource(string url)
        {
            return url.Contains("vine.co");
        }

        public override string CreateEmbed(string url, bool autoPlay = false)
        {
            var match = Regex.Match(url, REGEX_VINE);

            if (match.Success)
            {
                return ProcessTemplate(match.Groups[1]);
            }

            return new UnknownEmbed().CreateEmbed(url);
        }

        public override string GetThumbnail(string url)
        {
            return EmbedHelper.Config.DefaultVideoThumbnail;
        }
    }
}