using Infra.Crosscutting.Embed.Resources;
using System.Collections.Generic;

namespace Infra.Crosscutting.Embed
{
    public static class ResourceEmbedFactory
    {
        private static readonly IList<ResourceEmbed> Resources = new List<ResourceEmbed>
        {
            new ImageEmbed(),
            new YouTubeEmbed(),
            new FacebookEmbed(),
            new TwitterEmbed(),
            new InstagramEmbed(),
            new TwitchTvEmbed(),
            new VimeoEmbed(),
            new DailymotionEmbed(),
            new VineEmbed(),
            new VidmeEmbed(),
            new ImgurEmbed(),
            new VideoEmbed()
        };

        public static ResourceEmbed GetResource(string url)
        {
            foreach (var resource in Resources)
            {
                if (resource.IsResource(url))
                {
                    return resource;
                }
            }

            return new UnknownEmbed();
        }
    }
}