using System;
using System.IO;

namespace Infra.Crosscutting.Embed.Resources
{
    public class VideoEmbed : ResourceEmbed
    {
        private const string Template = "<video controls {2} loop=\"loop\"><source src=\"{0}\" type=\"video/{1}\"></video>";

        public VideoEmbed()
            : base(Template)
        {
        }

        public override bool IsResource(string url)
        {
            return url.IsVideoUrl();
        }

        public override string CreateEmbed(string url, bool autoPlay = false)
        {
            var fileName = new Uri(url).GetFileName();
            var extension = Path.GetExtension(fileName).Replace(".", "");

            return ProcessTemplate(url, extension, autoPlay ? "autoplay" : "");
        }

        public override string GetThumbnail(string url)
        {
            return EmbedHelper.Config.DefaultVideoThumbnail;
        }
    }
}