using Framework;
using PaymentIntegrator.Iugu.Common;
using RestSharp;
using System;
using System.Net;
using System.Text;

namespace PaymentIntegrator.Iugu
{
    public class IuguClient
    {
        private readonly string _resource;
        private readonly string _apiKey;

        public IuguClient(string resource)
            : this(resource, null)
        {
        }

        public IuguClient(string resource, string apiKey)
        {
            _resource = resource;
            _apiKey = apiKey ?? (IuguConfig.IsTest ? IuguConfig.ApiKeyTest : IuguConfig.ApiKeyLive);
        }

        private RestClient CreateClient(string baseUrl)
        {
            var client = new RestClient(baseUrl);

            // Override with Newtonsoft JSON Handler
            client.AddHandler("application/json", NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/json", NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/x-json", NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/javascript", NewtonsoftJsonSerializer.Default);
            client.AddHandler("*+json", NewtonsoftJsonSerializer.Default);

            return client;
        }

        private RestRequest CreateRequest(Method method)
        {
            var request = new RestRequest(_resource)
            {
                Method = method
            };

            return request;
        }

        private void SetContent(RestRequest request, object obj)
        {
            if (obj != null)
            {
                request.RequestFormat = DataFormat.Json;
                request.JsonSerializer = NewtonsoftJsonSerializer.Default;
                request.AddJsonBody(obj);
            }
        }

        private void SetToken(RestRequest request)
        {
            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(_apiKey)));
        }

        private OperationResult ProcessResponse(IRestResponse response)
        {
            var operationResult = new OperationResult();

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                operationResult.AddError("error", "Erro inexperado: " + response.ErrorMessage);
                return operationResult;
            }

            if (response.StatusCode.GetHashCode() == 422)
            {
                try
                {
                    var errorContent = NewtonsoftJsonSerializer.Default.Deserialize<IuguErrorListContent>(response);
                    errorContent.MergeErrors(operationResult);
                }
                catch (Exception)
                {
                    operationResult.AddError("error", "Erro inexperado: " + response.StatusDescription);
                }

                return operationResult;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                try
                {
                    var errorContent = NewtonsoftJsonSerializer.Default.Deserialize<IuguErrorContent>(response);
                    errorContent.MergeErrors(operationResult);
                }
                catch (Exception)
                {
                    operationResult.AddError("error", "Erro inexperado: " + response.StatusDescription);
                }

                return operationResult;
            }

            return OperationResult.Success;
        }

        private OperationResult<TResult> ProcessResponse<TResult>(IRestResponse response)
        {
            var operationResult = ProcessResponse(response);
            if (!operationResult.OK) return operationResult.Errors;

            var resultWithData = new OperationResult<TResult>(NewtonsoftJsonSerializer.Default.Deserialize<TResult>(response));
            return resultWithData;
        }

        public OperationResult<TResult> Post<TResult>(object obj)
        {
            var client = CreateClient(IuguConfig.BaseUrl);
            var request = CreateRequest(Method.POST);

            SetToken(request);
            SetContent(request, obj);

            var response = client.Execute(request);
            return ProcessResponse<TResult>(response);
        }

        public OperationResult<TResult> Put<TResult>(object obj)
        {
            var client = CreateClient(IuguConfig.BaseUrl);
            var request = CreateRequest(Method.PUT);

            SetToken(request);
            SetContent(request, obj);

            var response = client.Execute(request);
            return ProcessResponse<TResult>(response);
        }

        public OperationResult<TResult> Get<TResult>()
        {
            var client = CreateClient(IuguConfig.BaseUrl);
            var request = CreateRequest(Method.GET);

            SetToken(request);

            var response = client.Execute(request);
            return ProcessResponse<TResult>(response);
        }

        public OperationResult Delete()
        {
            var client = CreateClient(IuguConfig.BaseUrl);
            var request = CreateRequest(Method.DELETE);

            SetToken(request);

            var response = client.Execute(request);
            return ProcessResponse(response);
        }
    }
}