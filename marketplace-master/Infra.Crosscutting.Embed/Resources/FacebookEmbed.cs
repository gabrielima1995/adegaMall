using System;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Infra.Crosscutting.Embed.Resources
{
    public class FacebookEmbed : ResourceEmbed
    {
        private const string Template = "<script>(function(d, s, id) { var js, fjs = d.getElementsByTagName(s)[0]; if (d.getElementById(id)) return; js = d.createElement(s); js.id = id; js.src = \"//connect.facebook.net/pt_BR/sdk.js#xfbml=1&appId=524777684290169&version=v2.0\"; fjs.parentNode.insertBefore(js, fjs); }(document, \'script\', \'facebook-jssdk\'));</script>\n<div class=\"fb-post\" data-href=\"{0}\" data-width=\"466\"></div>";

        public FacebookEmbed()
            : base(Template)
        {
        }

        public override bool IsResource(string url)
        {
            return url.Contains("facebook.com") && (url.Contains("photo") || url.Contains("video") || url.Contains("post"));
        }

        public override string CreateEmbed(string url, bool autoPlay = false)
        {
            var embed = Template.Replace("{0}", url);
            return embed;
        }

        public override string GetThumbnail(string url)
        {
            var regex = new Regex(@"^http\S*facebook.com\S*videos?\/(\d*)\/?$");
            var match = regex.Match(url);

            var id = match.Groups[1].Value;

            string thumbUrl = EmbedHelper.Config.DefaultVideoThumbnail;
            using (var client = new HttpClient())
            {
                var response = client.GetAsync($"https://graph.facebook.com/{id}/picture").Result;
                if (response.IsSuccessStatusCode)
                    thumbUrl = response.RequestMessage.RequestUri.AbsoluteUri;
            }

            //var thumbUrl = "/assets/images/logo-facebook.png";

            return thumbUrl;
        }
    }
}