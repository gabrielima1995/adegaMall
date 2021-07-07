using System;
using System.IO;
using System.Web.Hosting;
using Serilog;
using Serilog.Core;

namespace Infrastructure
{
    public static class MarketplaceLogger
    {
        private static Logger _logger;

        public static Logger Instance
        {
            get
            {
                if (_logger == null)
                    _logger = new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .WriteTo.RollingFile(
                            Path.Combine(HostingEnvironment.MapPath("~/") ?? AppDomain.CurrentDomain.BaseDirectory,
                                "Logs", "marketplace-errors-{Date}.txt"))
                        .CreateLogger();

                return _logger;
            }
        }
    }
}