using Framework.Polly;
using Framework.Settings;
using Polly.Wrap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        protected ApiResourceBase(
            HttpClient client)
        {
            client.DefaultRequestHeaders.Add("User-Agent", "INA Marketplace v1");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _client = client;
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

            HttpRequestMessage CreateRequest()
            {
                return requestCreator.Invoke();
            }

            Task<HttpResponseMessage> SendAsync()
            {
                var request = CreateRequest();

                return _client.SendAsync(request);
            }

            try
            {
                using (var response = await ConfigurePolicy(endpointSettings)
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
            catch (Exception ex)
            {
                return ApiResponse<TResult>.Failure(CreateRequest(), ex);
            }
        }
    }
}