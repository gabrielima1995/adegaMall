using Framework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Portfolio.SocialMedias.Providers
{
    internal class SocialMediaFacebook : ISocialMediaProvider
    {
        private const string ChannelsEndpoint = "https://graph.facebook.com/v2.8/me?fields=accounts%7Busername%2Cid%2Clink%2Cfan_count%7D&access_token={0}";

        public async Task<OperationResult<IList<SocialMediaChannelViewModel>>> GetChannelsAsync(ExternalLoginInfo loginInfo)
        {
            var errors = new ErrorDictionary();
            var httpClient = new HttpClient();

            var accessToken = loginInfo.ExternalIdentity.FindFirst("urn:facebook:access_token");

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

            if (response.accounts != null)
            {
                IList<SocialMediaChannelViewModel> list = new List<SocialMediaChannelViewModel>();

                var data = (JArray)response.accounts.data;

                foreach (JToken item in data)
                {
                    list.Add(new SocialMediaChannelViewModel(
                        provider: loginInfo.Login.LoginProvider,
                        id: item["id"].ToString(),
                        userName: (item["username"] ?? item["id"]).ToString(),
                        link: item["link"].ToString(),
                        followers: Convert.ToInt32(item["fan_count"])));
                }

                return list
                    .OrderByDescending(i => i.Followers)
                    .ToList();
            }

            errors.Add("error", Thread.CurrentThread.GetStringLocalized("PAGE_FACEBOOK_ERROR_MESSAGE"));

            return errors;
        }
    }
}