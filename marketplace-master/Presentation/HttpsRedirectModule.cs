using Application.Config;
using System;
using System.Linq;
using System.Web;

namespace Presentation
{
    public class HttpsRedirectModule : IHttpModule
    {
        private static readonly string[] _ignoreHosts;

        private static readonly string[] _ignorePaths;

        static HttpsRedirectModule()
        {
            _ignoreHosts = new[]
            {
                "amazonaws.com",
                "azurewebsites.net",
            };

            _ignorePaths = new[]
            {
                "/assets",
                "/content",
                "/fonts",
                "/scripts",
                "/cloudimage"
            };
        }

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += Context_BeginRequest;
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            var config = MaaSConfig.Current;

            if (config == null)
                return;

            if (config.Environment.DisableAutoRedirect)
                return;

            var request = HttpContext.Current.Request;

            if (request.IsLocal || request.IsSecureConnection || request.Url.Scheme.Contains("https"))
                return;

            if (config.Environment.AppUrl == null || !config.Environment.AppUrl.StartsWith("https"))
                return;

            var host = request.Url.Host;

            if (_ignoreHosts.Any(x => host.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
                return;

            var path = request.Url.AbsolutePath;

            if (_ignorePaths.Any(x => path.StartsWith(x, StringComparison.OrdinalIgnoreCase)))
                return;

            var redirectUrl = "https" + request.Url.ToString().Substring(4);

            HttpContext.Current.Response.Redirect(redirectUrl, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}