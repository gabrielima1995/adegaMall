using System.Web.Mvc;
using System.Web.Routing;

namespace Presentation
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*path}", new { path = @"blog\/(.*)" });
            routes.IgnoreRoute("firebase-messaging-sw.js");

            routes.AppendTrailingSlash = true;
            routes.LowercaseUrls = true;

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "PublicProfileSeller",
                url: "p/{nickname}",
                defaults: new { controller = "Sellers", action = "PublicProfile" },
                namespaces: new string[] {
                    "Presentation.Controllers"
                }
            );

            routes.MapRoute(
                name: "LoadingShippingInfos",
                url: "product/loadingshippinginfos",
                defaults: new { controller = "Products", action = "LoadingShippingInfos" },
                namespaces: new string[] {
                    "Presentation.Controllers.Products"
                }
            );           
            
            routes.MapRoute(
                name: "PublicPageProduct",
                url: "product/{id}",
                defaults: new { controller = "Products", action = "PublicPage" },
                namespaces: new string[] {
                    "Presentation.Controllers.Products"
                }
            );

            routes.MapRoute(
                name: "SiteMap",
                url: "sitemap.xml",
                defaults: new { controller = "Sitemap", action = "Index" },
                namespaces: new string[]
                {
                    "Presentation.Controllers"
                }
            );

            routes.MapRoute(
                name: "WebAppManifest",
                url: "manifest.json",
                defaults: new { controller = "Manifest", action = "Index" },
                namespaces: new string[]
                {
                    "Presentation.Controllers"
                }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] {
                    "Presentation.Controllers",
                    "Presentation.Controllers.Products",
                    "Presentation.Controllers.Subscriptions"
                }
            );
        }
    }
}