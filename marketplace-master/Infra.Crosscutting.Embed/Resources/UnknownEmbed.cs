using System;

namespace Infra.Crosscutting.Embed.Resources
{
    public class UnknownEmbed : ResourceEmbed
    {
        public UnknownEmbed()
            : base("<a href=\"{0}\">{0}</a>")
        {
        }

        public override bool IsResource(string url)
        {
            return true;
        }

        public override string CreateEmbed(string url, bool autoPlay = false)
        {
            return ProcessTemplate(url);
        }

        public override string GetThumbnail(string url)
        {
            return EmbedHelper.Config.DefaultVideoThumbnail;
        }
    }
}