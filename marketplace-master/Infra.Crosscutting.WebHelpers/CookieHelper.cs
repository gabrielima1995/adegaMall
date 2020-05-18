using Microsoft.Owin;
using System.Net;

namespace System.Web
{
    public static class CookieHelper
    {
        private static HttpContextBase GetContext()
        {
            return new HttpContextWrapper(HttpContext.Current);
        }

        private static IOwinRequest GetRequest(HttpContextBase context)
        {
            return context.GetOwinContext().Request;
        }

        private static IOwinResponse GetResponse(HttpContextBase context)
        {
            return context.GetOwinContext().Response;
        }

        public static void RemoveCookie(this HttpContextBase context, string name, string domain = null)
        {
            GetResponse(context).Cookies.Delete(name, new CookieOptions
            {
                Domain = domain
            });
        }

        public static void RemoveCookie(string name, string domain = null)
        {
            RemoveCookie(GetContext(), name, domain);
        }

        public static T LoadCookie<T>(this HttpContextBase context, string name)
        {
            try
            {
                var value = GetRequest(context).Cookies[name];

                return value != null ? value.FromJson<T>() : default(T);
            }
            catch (Exception)
            {
                throw new CookieException();
            }
        }

        public static T LoadCookie<T>(string name)
        {
            return LoadCookie<T>(GetContext(), name);
        }

        public static void CreateCookie<T>(this HttpContextBase context, string name, T obj, string domain = null, int days = 365)
        {
            var value = obj.ToJson();

            GetResponse(context).Cookies.Append(name, value, new CookieOptions
            {
                Domain = domain,
                Expires = DateTime.UtcNow.AddDays(days)
            });
        }

        public static void CreateCookie<T>(string name, T obj, string domain = null, int days = 365)
        {
            CreateCookie<T>(GetContext(), name, obj, domain, days);
        }
    }
}