using Framework;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Modules.Portfolio.SocialMedias.Providers
{
    internal class SocialMediaYoutube : ISocialMediaProvider
    {
        public async Task<OperationResult<IList<SocialMediaChannelViewModel>>> GetChannelsAsync(ExternalLoginInfo loginInfo)
        {
            IList<SocialMediaChannelViewModel> list = new List<SocialMediaChannelViewModel>();

            var at = loginInfo.ExternalIdentity.FindFirst("urn:googleplus:access_token");
            var rt = loginInfo.ExternalIdentity.FindFirst("urn:googleplus:refresh_token");

            if (at == null || rt == null)
                throw new InvalidOperationException("Access / Refresh Token missing.");

            var token = new TokenResponse
            {
                AccessToken = at.Value,
                RefreshToken = rt.Value
            };

            var cred = new UserCredential(new GoogleAuthorizationCodeFlow(
                        new GoogleAuthorizationCodeFlow.Initializer()
                        {
                            ClientSecrets = new ClientSecrets()
                            {
                                ClientId = ConfigHelper.GetSetting("GOOGLE:CLIENT_ID"),
                                ClientSecret = ConfigHelper.GetSetting("GOOGLE:CLIENT_SECRET")
                            }
                        }
                    ),
                    loginInfo.Email,
                    token
                );

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApplicationName = this.GetType().ToString(),
                HttpClientInitializer = cred,
            });

            var searchListRequest = youtubeService.Channels.List("statistics,snippet");
            searchListRequest.Mine = true;
            searchListRequest.MaxResults = 50;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            foreach (var searchResult in searchListResponse.Items)
            {
                list.Add(new SocialMediaChannelViewModel(
                        provider: "YouTube",
                        id: searchResult.Id,
                        userName: searchResult.Snippet.Title,
                        link: string.Format("https://www.youtube.com/channel/{0}", searchResult.Id),
                        followers: Convert.ToInt32(searchResult.Statistics.SubscriberCount)));
            }

            return list
                .OrderByDescending(i => i.Followers)
                .ToList();
        }
    }
}