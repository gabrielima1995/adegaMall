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
    internal class SocialMediaInstagram : ISocialMediaProvider
    {
        private const string ChannelsEndpoint =
            "https://api.instagram.com/v1/users/self/?access_token={0}";

        public async Task<OperationResult<IList<SocialMediaChannelViewModel>>> GetChannelsAsync(ExternalLoginInfo loginInfo)
        {
            IList<SocialMediaChannelViewModel> list = new List<SocialMediaChannelViewModel>();
            var httpClient = new HttpClient();

            var accessToken = loginInfo.ExternalIdentity.FindFirst("urn:instagram:accesstoken");

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

            if (response.data != null)
            {
                var data = (JObject)response.data;

                list.Add(new SocialMediaChannelViewModel(
                        provider: loginInfo.Login.LoginProvider,
                        id: data["id"].ToString(),
                        userName: data["username"].ToString(),
                        link: string.Format("https://www.instagram.com/{0}/", data["username"]),
                        followers: Convert.ToInt32(data["counts"]["followed_by"])));
            }

            return new OperationResult<IList<SocialMediaChannelViewModel>>(list);
        }
    }
}