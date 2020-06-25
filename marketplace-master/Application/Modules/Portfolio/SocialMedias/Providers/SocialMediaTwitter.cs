using Framework;
using LinqToTwitter;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Modules.Portfolio.SocialMedias.Providers
{
    internal class SocialMediaTwitter : ISocialMediaProvider
    {
        public Task<OperationResult<IList<SocialMediaChannelViewModel>>> GetChannelsAsync(ExternalLoginInfo loginInfo)
        {
            IList<SocialMediaChannelViewModel> list = new List<SocialMediaChannelViewModel>();

            var oauthToken = loginInfo.ExternalIdentity.FindFirst("urn:twitter:oauthtoken");
            var oauthTokenSecret = loginInfo.ExternalIdentity.FindFirst("urn:twitter:oauthtokensecret");

            if (oauthToken == null)
            {
                throw new InvalidOperationException("OAuth token must be provided.");
            }

            if (oauthTokenSecret == null)
            {
                throw new InvalidOperationException("OAuth token secret must be provided.");
            }

            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore
                {
                    ConsumerKey = ConfigHelper.GetSetting("TWITTER:CONSUMER_KEY"),
                    ConsumerSecret = ConfigHelper.GetSetting("TWITTER:CONSUMER_SECRET"),
                    OAuthToken = oauthToken.Value,
                    OAuthTokenSecret = oauthTokenSecret.Value
                }
            };

            var twitterCtx = new TwitterContext(auth);

            var accounts =
                from acct in twitterCtx.Account
                where acct.Type == AccountType.VerifyCredentials
                select acct;

            var account = accounts.SingleOrDefault();
            var user = account?.User;

            if (user != null)
            {
                list.Add(new SocialMediaChannelViewModel(
                        provider: loginInfo.Login.LoginProvider,
                        id: user.UserIDResponse,
                        userName: user.ScreenNameResponse,
                        link: string.Format("https://www.twitter.com/{0}/", user.ScreenNameResponse),
                        followers: Convert.ToInt32(user.FollowersCount)));
            }

            return Task.FromResult(new OperationResult<IList<SocialMediaChannelViewModel>>(list));
        }
    }
}