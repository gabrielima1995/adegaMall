using ExpressiveAnnotations.MvcUnobtrusive.Providers;
using Infrastructure;
using Presentation.Globalization;
using Presentation.Helpers.CustomModelBinders;
using SqlServerTypes;
using System;
using System.Configuration;
using System.Data.Entity.SqlServer;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Presentation
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine { FileExtensions = new[] { "cshtml" } });

            Utilities.LoadNativeAssemblies(Server.MapPath("~/bin"));
            SqlProviderServices.SqlServerTypesAssemblyName = "Microsoft.SqlServer.Types, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";

            ModelBinders.Binders.Add(typeof(Application.Modules.Products.Search.SearchRequestViewModel), new ProductSearchRequestModelBinder());
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Globalization
            ModelValidatorProviders.Providers.Remove(ModelValidatorProviders.Providers.FirstOrDefault(x => x is DataAnnotationsModelValidatorProvider));
            ModelValidatorProviders.Providers.Add(new ExpressiveAnnotationsModelValidatorProvider());
            GlobalizationRegistration.RegisterBinders(ModelBinders.Binders);
            GlobalizationDataAnnotationsAdaptersHelper.RegisterAdapters();

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            AutoMapperConfig.RegisterMappings();
            Dapper.EntityFramework.Handlers.Register();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Request.SetCurrentCulture();
            SetResponseCompression();
        }

        private void Application_Error(object sender, EventArgs e)
        {
            Response.Filter = null;

            if (!Request.Url.IsLoopback)
            {
                var error = Server.GetLastError();

                ReportError(error);
                Response.Clear();
                Server.ClearError();

                var key = Guid.NewGuid().ToString("N");
                Session.Add(key, error);
                Response.Redirect("~/error?id=" + key);
            }
        }

        private void ReportError(Exception ex)
        {
            var reportingService = UnityConfig.Resolve<ErrorReportingService>();
            reportingService.ReportError(ex);
        }

        protected void SetResponseCompression()
        {
            if (ConfigurationManager.AppSettings["DisableCompression"] != null)
                return;

            var shouldCompress = !Request.Url.PathAndQuery.StartsWith("/api", StringComparison.OrdinalIgnoreCase);

            string encodings = Request.Headers.Get("Accept-Encoding");

            if (shouldCompress && encodings != null)
            {
                encodings = encodings.ToLower();

                if (encodings.Contains("deflate"))
                {
                    Response.Filter = new DeflateStream(Response.Filter, CompressionMode.Compress);
                    Response.AppendHeader("Content-Encoding", "deflate");
                }
                else if (encodings.Contains("gzip"))
                {
                    Response.Filter = new GZipStream(Response.Filter, CompressionMode.Compress);
                    Response.AppendHeader("Content-Encoding", "gzip");
                }
            }
        }
    }
}