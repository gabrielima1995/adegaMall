using Framework.Polly;
using Framework.Settings;
using Newtonsoft.Json;
using Polly.Wrap;
using Serilog.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Framework.ApiProviders
{
    public abstract class ApiResourceBase
    {
        private static readonly ConcurrentDictionary<string, AsyncPolicyWrap<HttpResponseMessage>> Policies =
            new ConcurrentDictionary<string, AsyncPolicyWrap<HttpResponseMessage>>();

        private readonly HttpClient _client;
        private readonly Logger _logger;

        protected ApiResourceBase(
            HttpClient client, Logger logger)
        {
            client.DefaultRequestHeaders.Add("User-Agent", "INA Marketplace v1");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _client = client;
            _logger = logger;
        }

        private AsyncPolicyWrap<HttpResponseMessage> ConfigurePolicy(EndpointSettings endpointSettings)
        {
            return Policies.GetOrAdd(
                endpointSettings.Key,
                HttpPolicyHelper.ResilienceStrategy(
                    endpointSettings.RetrySleeps,
                    endpointSettings.EventsBeforeBreaking,
                    endpointSettings.DurationOfBreak,
                    endpointSettings.Timeout));
        }

        protected abstract HttpContent CreateContent<TCommand>(TCommand command);

        protected abstract Task<TResult> ParseSuccessResponseAsync<TResult>(HttpResponseMessage response);

        protected abstract Task<List<ApiErrorMessage>> ParseErrorResponseAsync(HttpResponseMessage response);

        protected async Task<ApiResponse<TResult>> SendAsync<TCommand, TResult>(EndpointSettings endpointSettings,
            Func<HttpRequestMessage> requestCreator,
            TCommand command,
            Func<HttpResponseMessage, Task<List<ApiErrorMessage>>> errorResponseProcessor = null)
            where TResult : class
        {
            if (endpointSettings is null) throw new ArgumentNullException(nameof(endpointSettings));
            if (requestCreator is null) throw new ArgumentNullException(nameof(requestCreator));
            if (command == null) throw new ArgumentNullException(nameof(command));

            var content = CreateContent(command);

            HttpRequestMessage CreateRequest()
            {
                var request = requestCreator.Invoke();

                request.Content = content;

                return request;
            }

            return await SendAsync<TResult>(endpointSettings, CreateRequest, errorResponseProcessor)
                .ConfigureAwait(false);
        }

        protected async Task<ApiResponse<TResult>> SendAsync<TResult>(EndpointSettings endpointSettings,
            Func<HttpRequestMessage> requestCreator,
            Func<HttpResponseMessage, Task<List<ApiErrorMessage>>> errorResponseProcessor = null)
            where TResult : class
        {
            if (endpointSettings is null) throw new ArgumentNullException(nameof(endpointSettings));
            if (requestCreator is null) throw new ArgumentNullException(nameof(requestCreator));

            HttpRequestMessage request = null;
            HttpResponseMessage response = null;
            Exception ex = null;

            Task<HttpResponseMessage> SendAsync()
            {
                request = requestCreator.Invoke();

                return _client.SendAsync(request);
            }

            var stopWatch = new Stopwatch();

            try
            {
                stopWatch.Start();

                using (response = await ConfigurePolicy(endpointSettings)
                    .ExecuteAsync(SendAsync)
                    .ConfigureAwait(false))
                {
                    if (response.IsSuccessStatusCode)
                        return await ApiResponse<TResult>
                            .SuccessAsync(response, ParseSuccessResponseAsync<TResult>)
                            .ConfigureAwait(false);

                    return await ApiResponse<TResult>.FailureAsync(response,
                            errorResponseProcessor ?? ParseErrorResponseAsync)
                        .ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                ex = e;
                return ApiResponse<TResult>.Failure(request, ex);
            }
            finally
            {
                stopWatch.Stop();

                await LogAsync(request, response, stopWatch.ElapsedMilliseconds, ex);
            }
        }

        private async Task LogAsync(HttpRequestMessage request, HttpResponseMessage response, long durationMs, Exception ex)
        {
            try
            {
                string requestContentBody = null;

                if (request.Content != null)
                {
                    await request.Content
                        .ReadAsStringAsync()
                        .ContinueWith(task =>
                        {
                            requestContentBody = task.Result;
                        });
                }

                var requestToLog = new
                {
                    uri = request.RequestUri.ToString(),
                    method = request.Method.Method,
                    token = request.Headers.Authorization?.Scheme + " " + request.Headers.Authorization?.Parameter,
                    headers = request.Headers.ToDictionary(),
                    content = requestContentBody
                };

                string responseContentBody = null;

                if (response?.Content != null)
                {
                    await response.Content
                        .ReadAsStringAsync()
                        .ContinueWith(task =>
                        {
                            responseContentBody = task.Result;
                        });
                }

                var responseToLog = new
                {
                    statusCode = response?.StatusCode ?? 0,
                    headers = response?.Content?.Headers.ToDictionary(),
                    contentType = response?.Content?.Headers.ContentType.MediaType,
                    content = responseContentBody,
                    errorMessage = ex?.Message,
                    stackTrace = ex?.StackTrace
                };

                _logger.Information($"Request completed in {durationMs} ms");
                _logger.Information($"Request: {JsonConvert.SerializeObject(requestToLog)}");
                _logger.Information($"Response: {JsonConvert.SerializeObject(responseToLog)}");
            }
            catch
            {
            }
        }
    }
}