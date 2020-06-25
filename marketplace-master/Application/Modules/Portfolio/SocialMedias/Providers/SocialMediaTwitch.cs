using Framework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Application.Modules.Portfolio.SocialMedias.Providers
{
    internal class SocialMediaTwitch : ISocialMediaProvider
    {
        private const string ChannelsEndpoint = "https://api.twitch.tv/kraken/channel/?oauth_token={0}";

        public async Task<OperationResult<IList<SocialMediaChannelViewModel>>> GetChannelsAsync(ExternalLoginInfo loginInfo)
        {
            IList<SocialMediaChannelViewModel> list = new List<SocialMediaChannelViewModel>();
            var httpClient = new HttpClient();

            var accessToken = loginInfo.ExternalIdentity.FindFirst("urn:Twitch:access_token");

            if (accessToken == null)
            {
                throw new InvalidOperationException("Access token must be provided.");
            }

            // Request the channels
            var channelsResponse = await httpClient.GetAsync(string.Format(ChannelsEndpoint, accessToken.Value));

            channelsResponse.EnsureSuccessStatusCode();

            var text = await channelsResponse.Content.ReadAsStringAsync();

            // Deserializes the token response
            dynamic response = JsonConvert.DeserializeObject<dynamic>(text);

            if (response != null)
            {
                var data = (JObject)response;

                list.Add(new SocialMediaChannelViewModel(
                    provider: loginInfo.Login.LoginProvider,
                    id: data["_id"].ToString(),
                    userName: data["name"].ToString(),
                    link: data["url"].ToString(),
                    followers: Convert.ToInt32(data["followers"])));
            }

            return new OperationResult<IList<SocialMediaChannelViewModel>>(list);
        }
    }
}