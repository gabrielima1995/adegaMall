using Hangfire;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Infrastructure.Logging
{
    /// <summary>
    /// The API Log Handler.
    /// </summary>
    public class ApiLogHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = (HttpContextBase)request.Properties["MS_HttpContext"];

            var apiLogRequest = await ApiLogRequest.NewAsync(request, context, cancellationToken);
            var response = await base.SendAsync(request, cancellationToken);
            var apiLogResponse = await ApiLogResponse.NewAsync(response, cancellationToken);

            if (ShouldLogRequest(request))
            {
                var apiLogEntry = new ApiLogEntry(Environment.MachineName, apiLogRequest, apiLogResponse);
                JobHelper.Enqueue<ApiLogService>(x => x.Save(apiLogEntry));
            }

            return response;
        }

        private bool ShouldLogRequest(HttpRequestMessage request)
        {
            var path = request.RequestUri.LocalPath;

            var ignore = new[]
            {
                "/api/upload",
                "/api/v1/cities"
            };

            foreach (var item in ignore)
                if (path.StartsWith(item, StringComparison.OrdinalIgnoreCase))
                    return false;

            return true;
        }
    }
}