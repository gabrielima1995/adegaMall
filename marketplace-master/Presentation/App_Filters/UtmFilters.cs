using Domain;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation
{
    public class RegisterUtmConversionFilter : UtmFilterAttributeBase
    {
        private readonly Lazy<PersonService> personService = new Lazy<PersonService>(() => UnityConfig.Resolve<PersonService>());

        protected override void OnActionExecutingCore(ActionExecutingContext filterContext)
        {
            var utmTags = GetUtmTags(filterContext);
            var username = GetUsernameOrNull(filterContext);

            if (utmTags == null || username == null || NotConversionPage(filterContext))
                return;

            personService.Value.SetUtmTagsIfNull(username, utmTags);
            RemoveUtmCookie(filterContext);
        }

        private bool NotConversionPage(ActionExecutingContext filterContext)
        {
            var path = filterContext.HttpContext.Request.Url.PathAndQuery;
            var ignoreCase = StringComparison.OrdinalIgnoreCase;
            var isConversionPage =
                path.StartsWith("/register/buyerFirstStepOk", ignoreCase) ||
                path.StartsWith("/register/buyerCompleted", ignoreCase) ||
                path.StartsWith("/register/sellerFirstStepOk", ignoreCase) ||
                path.StartsWith("/register/sellerCompleted", ignoreCase) ||
                path.StartsWith("/quotes/requestConfirmed", ignoreCase);
            return !isConversionPage;
        }

        private void RemoveUtmCookie(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.Cookies[cookieName] != null)
                filterContext.HttpContext.Response.Cookies[cookieName].Expires = DateTime.Now.AddDays(-1);
        }

        private string GetUtmTags(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;

            if (HasUtmParameters(request.QueryString))
                return UtmParameters.From(request.QueryString).Serialize();

            if (request.Cookies[cookieName] != null)
                return request.Cookies[cookieName].Value;

            return null;
        }

        private string GetUsernameOrNull(ActionExecutingContext filterContext)
        {
            return filterContext.HttpContext.User.Identity.IsAuthenticated ?
                filterContext.HttpContext.User.Identity.Name : null;
        }
    }

    public class CaptureUtmFilter : UtmFilterAttributeBase
    {
        protected override void OnActionExecutingCore(ActionExecutingContext filterContext)
        {
            var queryString = filterContext.HttpContext.Request.QueryString;

            if (HasUtmParameters(queryString))
            {
                var utm = UtmParameters.From(queryString);
                SetUtmCookie(filterContext.HttpContext.Response, utm.Serialize());
            }
        }

        private void SetUtmCookie(HttpResponseBase response, string cookieContent)
        {
            response.SetCookie(new HttpCookie(cookieName, cookieContent)
            {
                Secure = false,
                Path = "/",
                Expires = DateTime.Today.AddDays(30)
            });
        }
    }

    public abstract class UtmFilterAttributeBase : ActionFilterAttribute, IActionFilter
    {
        protected const string cookieName = "utm_parameters";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ExecuteOncePerRequest(filterContext);
        }

        protected bool HasUtmParameters(NameValueCollection queryString)
        {
            return queryString.AllKeys.Any(key => key != null && key.StartsWith("utm", StringComparison.OrdinalIgnoreCase));
        }

        private void ExecuteOncePerRequest(ActionExecutingContext filterContext)
        {
            if (!filterContext.IsChildAction)
                OnActionExecutingCore(filterContext);
        }

        protected abstract void OnActionExecutingCore(ActionExecutingContext filterContext);
    }

    internal class UtmParameters
    {
        private readonly string source;
        private readonly string medium;
        private readonly string campaign;
        private readonly string term;
        private readonly string content;

        private UtmParameters(string source, string medium, string campaign, string term, string content)
        {
            this.source = source;
            this.medium = medium;
            this.campaign = campaign;
            this.term = term;
            this.content = content;
        }

        public static UtmParameters From(NameValueCollection queryString)
        {
            return new UtmParameters(
                GetParameterOrNull(queryString, "utm_source"),
                GetParameterOrNull(queryString, "utm_medium"),
                GetParameterOrNull(queryString, "utm_campaign"),
                GetParameterOrNull(queryString, "utm_term"),
                GetParameterOrNull(queryString, "utm_content"));
        }

        private static string GetParameterOrNull(NameValueCollection queryString, string parameterName)
        {
            if (string.IsNullOrEmpty(queryString[parameterName])) return null;
            return queryString[parameterName];
        }

        public string Serialize()
        {
            var parts = new List<string>();

            if (source != null)
                parts.Add($"utm_source={HttpUtility.UrlEncode(source)}");

            if (medium != null)
                parts.Add($"utm_medium={HttpUtility.UrlEncode(medium)}");

            if (campaign != null)
                parts.Add($"utm_campaign={HttpUtility.UrlEncode(campaign)}");

            if (term != null)
                parts.Add($"utm_term={HttpUtility.UrlEncode(term)}");

            if (content != null)
                parts.Add($"utm_content={HttpUtility.UrlEncode(content)}");

            return string.Join("&", parts);
        }
    }
}