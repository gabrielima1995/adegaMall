using Application.Config;
using Domain.Interfaces;
using Domain.Messages;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Mvc;

namespace Infrastructure
{
    public class ErrorReportingService
    {
        private readonly IEmailSender _emailSender;

        public ErrorReportingService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public void ReportError(Exception ex)
        {
            if (ShouldSkipException(ex))
                return;

            if (IsCrawlerOrBot())
                return;

            MarketplaceLogger.Instance.Information("Error: " + ex.Message);
            if (ex.InnerException != null)
                MarketplaceLogger.Instance.Information("Inner: " + ex.InnerException.Message);
            MarketplaceLogger.Instance.Information("StackTrace: " + ex.StackTrace);

            var error = new
            {
                data = ex,
                datetime = DateTime.UtcNow,
                request_data = GetRequestData()
            };

            var settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var errorString = JsonConvert.SerializeObject(error, Formatting.None, settings);

            MarketplaceLogger.Instance.Information("Data: " + errorString);
        }

        private object GetRequestData()
        {
            if (HttpContext.Current == null) return null;
            var ctx = HttpContext.Current;

            return new
            {
                request = ctx.Request == null ? null : new
                {
                    http_method = ctx.Request.HttpMethod,
                    url = ctx.Request.Url?.ToString(),
                    url_referrer = ctx.Request.UrlReferrer?.ToString(),
                    user_agent = ctx.Request.UserAgent
                },
                session = ctx.Session == null ? null : new
                {
                    session_id = ctx.Session.SessionID
                },
                user = ctx.User == null ? null : new
                {
                    is_authenticated = ctx.User.Identity?.IsAuthenticated ?? false,
                    name = ctx.User.Identity?.Name
                }
            };
        }

        private bool IsCrawlerOrBot()
        {
            if (HttpContext.Current?.Request == null) return false;
            var ctx = HttpContext.Current;

            var agents = new[]
            {
                "facebot 1.0",
                "googlebot",
                "wordpress",
                "google-xrawler"
            };

            foreach (var a in agents)
                if (ctx.Request.UserAgent != null && ctx.Request.UserAgent.Contains(a, StringComparison.OrdinalIgnoreCase))
                    return true;

            return false;
        }

        private bool ShouldSkipException(Exception ex)
        {
            if (ex == null)
                return true;

            if (ex is HttpException && (ex as HttpException).GetHttpCode() == 404)
                return true;

            if (ex is HttpAntiForgeryException)
                return true;

            return false;
        }

        private void SendEmail(string subject, string error)
        {
            var emailTitle = $"Error at {MaaSConfig.Current.ApplicationName}: {subject}";

            if (emailTitle.Length > 70)
                emailTitle = emailTitle.Substring(0, 70);

            var htmlBody = error == null ? error : "<pre>" + error
                .Replace("\n", "<br/>")
                .Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;") + "</pre>";

            _emailSender.Send(new EmailMessage(
                Domain.Resources.AppResources.SYS_ERROR_REPORT_FROM_ADDRESS,
                new[] { Domain.Resources.AppResources.SYS_ERROR_REPORT_TO_ADDRESS },
                emailTitle, htmlBody));
        }
    }
}