using System;

namespace Infra.Crosscutting.Embed.Resources
{
    public class ImageEmbed : ResourceEmbed
    {
        private const string Template = "<img src=\"{0}\">";

        public ImageEmbed()
            : base(Template)
        {
        }

        public override bool IsResource(string url)
        {
            return url.IsImageUrl();
        }

        public override string CreateEmbed(string url, bool autoPlay = false)
        {
            return ProcessTemplate(url);
        }

        public override string GetThumbnail(string url)
        {
            return url;
        }
    }
}