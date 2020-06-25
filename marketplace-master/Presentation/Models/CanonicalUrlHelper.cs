using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Application.Config;
using Se.Url;

namespace Presentation.Models
{
    public static class CanonicalUrlHelper
    {
        public static string Canonical(this HttpRequestBase request)
        {
            var host = MaaSConfig.Current.Environment.AppUrl;

            var builder = new UrlBuilder(host + request.RawUrl);

            builder.RemoveQueryParams("lang", "Lang", "utm_source", "utm_medium", "utm_campaign", "utm_term", "utm_content");

            return builder.ToString();
        }
    }
}