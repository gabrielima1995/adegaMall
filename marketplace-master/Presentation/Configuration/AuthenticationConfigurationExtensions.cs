using Application.Config;
using Domain.Entities;
using Infrastructure.Data.Context;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Facebook;
using Owin;
using System;
using Unity;

namespace Presentation.Configuration
{
    public static class AuthenticationConfigurationExtensions
    {
        private static bool firstTimeCall = true;
        private static IAppBuilder app;

        public static IAppBuilder UseAuthentication(this IAppBuilder app)
        {
            AuthenticationConfigurationExtensions.app = app;
            MaaSConfig.Loaded += MaaSConfig_Loaded;

            return app;
        }

        private static void MaaSConfig_Loaded(object sender, EventArgs e)
        {
            if (firstTimeCall)
            {
                firstTimeCall = false;
                ConfigureAuth(app);
            }
        }

        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }

        private static void ConfigureAuth(IAppBuilder app)
        {
            var container = UnityConfig.GetConfiguredContainer();

            DataProtectionProvider = app.GetDataProtectionProvider();

            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(() => container.Resolve<ScaffoldContext>());
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.Use((context, next) =>
            {
                var isHttps = MaaSConfig.Current.Environment.AppUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase);
                var facebookEnabled = !MaaSConfig.Current.Facebook.DisableLoginForBuyer || !MaaSConfig.Current.Facebook.DisableLoginForSeller;
                if (isHttps && facebookEnabled) context.Request.Scheme = "https";
                return next();
            });

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager)),

                    OnApplyRedirect = ctx =>
                    {
                        var response = ctx.Response;

                        if (!IsApiResponse(ctx.Response))
                            response.Redirect(ctx.RedirectUri);
                    }
                },
                //CookieManager = new SystemWebCookieManager()
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            //app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            //app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            #region Twitter

            //var twitterOptions = new TwitterAuthenticationOptions
            //{
            //    ConsumerKey = ConfigHelper.GetSetting("TWITTER:CONSUMER_KEY"),
            //    ConsumerSecret = ConfigHelper.GetSetting("TWITTER:CONSUMER_SECRET"),
            //    Provider = new TwitterAuthenticationProvider
            //    {
            //        OnAuthenticated = (context) =>
            //        {
            //            context.Identity.AddClaim(new Claim("urn:twitter:oauthtoken", context.AccessToken));
            //            context.Identity.AddClaim(new Claim("urn:twitter:oauthtokensecret", context.AccessTokenSecret));

            //            return Task.FromResult(0);
            //        }
            //    },
            //    BackchannelCertificateValidator =
            //        new Microsoft.Owin.Security.CertificateSubjectKeyIdentifierValidator(new[]
            //        {
            //            "A5EF0B11CEC04103A34A659048B21CE0572D7D47", // VeriSign Class 3 Secure Server CA - G2
            //            "0D445C165344C1827E1D20AB25F40163D8BE79A5", // VeriSign Class 3 Secure Server CA - G3
            //            "7FD365A7C2DDECBBF03009F34339FA02AF333133", // VeriSign Class 3 Public Primary Certification Authority - G5
            //            "39A55D933676616E73A761DFA16A7E59CDE66FAD", // Symantec Class 3 Secure Server CA - G4
            //            "‎add53f6680fe66e383cbac3e60922e3b4c412bed", // Symantec Class 3 EV SSL CA - G3
            //            "4eb6d578499b1ccf5f581ead56be3d9b6744a5e5", // VeriSign Class 3 Primary CA - G5
            //            "5168FF90AF0207753CCCD9656462A212B859723B", // DigiCert SHA2 High Assurance Server C‎A
            //            "B13EC36903F8BF4701D498261A0802EF63642BC3" // DigiCert High Assurance EV Root CA
            //        })
            //};

            //app.UseTwitterAuthentication(twitterOptions);

            #endregion Twitter

            #region Facebook

            var facebookOptions = new FacebookAuthenticationOptions
            {
                AppId = MaaSConfig.Current.Facebook.AppId,
                AppSecret = MaaSConfig.Current.Facebook.AppSecret,
                /*,
                UserInformationEndpoint = "https://graph.facebook.com/v" + ConfigHelper.GetSetting("FACEBOOK:API:VERSION", "2.8") + "/me?fields=id,name,email,picture.type(large),accounts{username,id,link,fan_count}",
                BackchannelHttpHandler = new FacebookBackChannelHandler(),
                Provider = new FacebookAuthenticationProvider()
                {
                    OnAuthenticated = (context) =>
                    {
                        if (!string.IsNullOrEmpty(context.Email))
                        {
                            context.Identity.AddClaim(new Claim("urn:facebook:email", context.Email));
                        }

                        context.Identity.AddClaim(new Claim("urn:facebook:access_token", context.AccessToken));
                        context.Identity.AddClaim(new Claim("urn:facebook:id", context.Id));
                        context.Identity.AddClaim(new Claim("urn:facebook:name", context.Name));
                        context.Identity.AddClaim(new Claim("urn:facebook:picture", context.User["picture"]["data"].Value<string>("url")));

                        return Task.FromResult(0);
                    }
                }*/
            };

            facebookOptions.Scope.Add("email");

            app.UseFacebookAuthentication(facebookOptions);
            #endregion Facebook

            #region Google Plus

            //var googlePlusOptions = new GooglePlusAuthenticationOptions();
            //googlePlusOptions.ClientId = ConfigHelper.GetSetting("GOOGLE:CLIENT_ID");
            //googlePlusOptions.ClientSecret = ConfigHelper.GetSetting("GOOGLE:CLIENT_SECRET");
            //googlePlusOptions.RequestOfflineAccess = true;  // for refresh token
            //googlePlusOptions.Provider = new GooglePlusAuthenticationProvider
            //{
            //    OnAuthenticated = context =>
            //    {
            //        context.Identity.AddClaim(new Claim("urn:googleplus:access_token", context.AccessToken));

            //        if (!string.IsNullOrEmpty(context.RefreshToken))
            //        {
            //            context.Identity.AddClaim(new Claim("urn:googleplus:refresh_token", context.RefreshToken));
            //        }

            //        return Task.FromResult(0);
            //    }
            //};

            //googlePlusOptions.Scope.Add("https://www.googleapis.com/auth/youtube.readonly");
            //googlePlusOptions.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
            //app.UseGooglePlusAuthentication(googlePlusOptions);

            #endregion Google Plus

            #region Instagram

            //var instagramOptions = new InstagramAuthenticationOptions
            //{
            //    ClientId = ConfigHelper.GetSetting("INSTAGRAM:CLIENT_ID"),
            //    ClientSecret = ConfigHelper.GetSetting("INSTAGRAM:CLIENT_SECRET")
            //};

            //app.UseInstagramInAuthentication(instagramOptions);

            #endregion Instagram

            #region Twitch

            //var twitchOptions = new TwitchAuthenticationOptions
            //{
            //    ClientId = ConfigHelper.GetSetting("TWITCH:CLIENT_ID"),
            //    ClientSecret = ConfigHelper.GetSetting("TWITCH:CLIENT_SECRET"),
            //    Provider = new TwitchAuthenticationProvider
            //    {
            //        OnAuthenticated = context =>
            //        {
            //            context.Identity.AddClaim(new Claim("urn:Twitch:access_token", context.AccessToken));

            //            return Task.FromResult(0);
            //        }
            //    }
            //};

            //twitchOptions.Scope.Add("channel_read");

            //app.UseTwitchAuthentication(twitchOptions);

            #endregion Twitch
        }

        private static bool IsApiResponse(IOwinResponse response)
        {
            var responseHeader = response.Headers;
            if (responseHeader == null)
                return false;

            return responseHeader["Suppress-Redirect"] == "True";
        }
    }
}