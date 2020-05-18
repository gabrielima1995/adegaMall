using Infra.Crosscutting.Embed;

namespace System
{
    public static class EmbedHelper
    {
        public static EmbedConfig Config { get; set; } = new EmbedConfig();

        public static string CreateEmbed(this string url, bool autoPlay = false)
        {
            var resource = ResourceEmbedFactory.GetResource(url);

            return resource.CreateEmbed(url, autoPlay);
        }

        public static string GetThumbnail(this string url)
        {
            var resource = ResourceEmbedFactory.GetResource(url);

            return resource.GetThumbnail(url);
        }
    }
}