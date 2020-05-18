using System;
using System.Text.RegularExpressions;

namespace Infra.Crosscutting.Embed.Resources
{
    public class TwitchTvEmbed : ResourceEmbed
    {
        private const string Template = "<object bgcolor='#000000' data='http://www.twitch.tv/widgets/archive_embed_player.swf' height='378' id='clip_embed_player_flash' type='application/x-shockwave-flash' width='620'><param name='movie' value='http://www.twitch.tv/widgets/archive_embed_player.swf' /><param name='allowScriptAccess' value='always' /><param name='allowNetworking' value='all' /><param name='allowFullScreen' value='true' /><param name='flashvars' value='channel={0}&amp;auto_play=false&amp;start_volume=25&amp;{1}' /></object>";
        private const string REGEX_TWITCH = @"^.+twitch.tv\/(\w*)(\/c\/(.*))?";

        public TwitchTvEmbed()
            : base(Template)
        {
        }

        public override bool IsResource(string url)
        {
            return url.Contains("twitch.tv");
        }

        public override string CreateEmbed(string url, bool autoPlay = false)
        {
            var match = Regex.Match(url, REGEX_TWITCH);

            if (match.Success)
            {
                return ProcessTemplate(match.Groups[1], match.Groups.Count > 3 ? "chapter_id=" + match.Groups[3] : "");
            }

            return new UnknownEmbed().CreateEmbed(url);
        }

        public override string GetThumbnail(string url)
        {
            return EmbedHelper.Config.DefaultVideoThumbnail;
        }
    }
}