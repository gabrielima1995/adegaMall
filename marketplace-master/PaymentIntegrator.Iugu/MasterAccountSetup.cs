using Framework;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;

namespace PaymentIntegrator.Iugu
{
    public static class MasterAccountSetup
    {
        public static OperationResult Configure()
        {
            var request = GetRequest();
            var client = GetClient();
            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                return new OperationResult("Error", response.StatusCode + " " + response.StatusDescription + ": " + response.Content);

            return OperationResult.Success;
        }

        private static RestClient GetClient()
        {
            var apiToken = IuguConfig.ApiKeyLive; // Configuration of the master account must always use the live api token.
            var client = new RestClient("https://api.iugu.com/v1")
            {
                Authenticator = new HttpBasicAuthenticator(apiToken, "")
            };

            return client;
        }

        private static RestRequest GetRequest()
        {
            var request = new RestRequest("accounts/configuration", Method.POST);

            request.AddJsonBody(new
            {
                back_slip = new
                {
                    active = true
                },
                credit_card = new
                {
                    active = true,
                    soft_descriptor = IuguConfig.SoftDescriptor,
                    installments = true,
                    max_installments = 12
                }
            });

            return request;
        }
    }
}
