using Framework;
using System;
using System.Web;

namespace Presentation.Helpers
{
    public static class WishlistHelper
    {
        private const string CookieName_WishlistAdd = "__WISHLIST_AD";

        public static void StoreItemForLogin(Guid productId)
        {
            var value = Base64.Encode($"{productId},");

            var cookie = new HttpCookie(CookieName_WishlistAdd, value)
            {
                Expires = DateTime.UtcNow.AddHours(1D)
            };

            HttpContext.Current.Response.Cookies.Set(cookie);
        }

        public static bool RetrieveItemAfterLogin(out Guid productId)
        {
            productId = Guid.Empty;

            var cookie = HttpContext.Current.Request.Cookies[CookieName_WishlistAdd];

            if (cookie != null)
            {
                try
                {
                    var raw = cookie.Value;
                    cookie.Expires = DateTime.Now.AddDays(-1D);
                    HttpContext.Current.Response.Cookies.Remove(CookieName_WishlistAdd);
                    HttpContext.Current.Response.Cookies.Set(cookie);

                    var parts = Base64.Decode(raw).Split(',');

                    productId = Guid.Parse(parts[0]);

                    return true;
                }
                catch { }
            }

            return false;
        }
    }
}