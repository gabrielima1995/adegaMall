using PaymentIntegrator.Moip.Common;
using PaymentIntegrator.Moip.Common.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;

namespace PaymentIntegrator.Moip
{
    public class MoipClient
    {
        private readonly RestClient restClient;

        private MoipClient()
        {
            var baseUrl = new Uri(MoipConfig.Current.IsSandbox
                ? "https://sandbox.moip.com.br/v2/"
                : "https://api.moip.com.br/v2/");

            restClient = new RestClient(baseUrl)
            {
                Authenticator = new HttpBasicAuthenticator(MoipConfig.Current.AccountToken, MoipConfig.Current.AccountKey)
            };
        }

        /// <summary>
        /// Request using Basic Authentication with Marketplace Tokens
        /// </summary>
        public static IRestResponse SendRequest(MoipRequest request)
        {
            var client = new MoipClient();
            return client.restClient.Execute(request);
        }

        /// <summary>
        /// OAuth Request using the Marketplace Access Token
        /// </summary>
        public static IRestResponse SendOAuthRequest(MoipRequest request)
        {
            if (request is CreateMultiOrderRequest || request is CreateMultiPaymentRequest || request is GetPaymentRequest)
                throw new InvalidOperationException("This method is being refactored. Use the Send method, which accepts a typed Request.");

            return SendOAuthRequestInternal(request);
        }

        public static CreateMultiOrderResult Send(CreateMultiOrderRequest request)
        {
            var restResponse = SendOAuthRequestInternal(request);
            return CreateMultiOrderResult.Create(restResponse);
        }

        public static CreateMultiPaymentResult Send(CreateMultiPaymentRequest request)
        {
            var restResponse = SendOAuthRequestInternal(request);
            return CreateMultiPaymentResult.Create(restResponse);
        }

        public static GetMultiPaymentResult Send(GetPaymentRequest request)
        {
            var restResponse = SendOAuthRequestInternal(request);
            return GetMultiPaymentResult.Create(restResponse);
        }

        public static IRestResponse SendOAuthRequest(MoipRequest request, string accessToken)
        {
            var client = new MoipClient();
            client.restClient.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(accessToken);

            return client.restClient.Execute(request);
        }

        public static IRestResponse SendV1Request(MoipRequest request)
        {
            var client = new MoipClient();
            client.restClient.BaseUrl = new Uri(client.restClient.BaseUrl.ToString().Replace("/v2/", "/assinaturas/v1/"));

            var response = client.restClient.Execute(request);
            return response;
        }

        private static IRestResponse SendOAuthRequestInternal(MoipRequest request)
        {
            var client = new MoipClient();
            client.restClient.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(MoipConfig.Current.AppAccessToken);
            return client.restClient.Execute(request);
        }
    }
}