using Infra.Crosscutting.WebHelpers.WebApi.Filters;
using Infrastructure.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Presentation
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;

            if (ConfigurationManager.AppSettings["ApiCorsEnabled"] == "true")
            {
                string origins = "*";

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ApiCorsOrigins"]))
                    origins = ConfigurationManager.AppSettings["ApiCorsOrigins"];

                config.EnableCors(new EnableCorsAttribute(origins, "*", "*"));
            }


            #region Configures formatters

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new SnakeCasePropertyNamesContractResolver();

            jsonFormatter.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            jsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            jsonFormatter.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;

            jsonFormatter.SerializerSettings.Converters.Add(new StringSnakeCaseEnumConverter());
            jsonFormatter.SerializerSettings.Converters.Add(new DbGeographyConverter());
            jsonFormatter.SerializerSettings.Converters.Add(new DbGeographyEFConverter());
            jsonFormatter.SerializerSettings.Converters.Add(new CustomIsoDateTimeConverter());

            #endregion Configures formatters

#if !DEBUG
            //config.MessageHandlers.Insert(0, new ServerCompressionHandler(new GZipCompressor(), new DeflateCompressor()));
#endif

            #region Adds filters

            config.Filters.Add(new ErrorLoggingFilterAttribute());
            config.Filters.Add(new ValidateModelFilterAttribute());

            #endregion Adds filters

            config.MapHttpAttributeRoutes();

            //config.MessageHandlers.Add(new SuppressRedirectHandler());
            config.MessageHandlers.Add(new ApiLogHandler());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}