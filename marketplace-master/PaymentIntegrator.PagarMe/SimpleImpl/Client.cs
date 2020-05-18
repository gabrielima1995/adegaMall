using RestSharp;
using RestSharp.Authenticators;
using System;

namespace PaymentIntegrator.PagarMe.SimpleImpl
{
    public class Client
    {
        public Response Execute(Request request)
        {
            var auth = new HttpBasicAuthenticator(Config.ApiKey, "x");
            var restClient = new RestClient(Config.BasePath) { Authenticator = auth };
            var restRequest = new RestRequest(request.Endpoint, GetRestMethod(request));
            var restResponse = restClient.Execute(restRequest);
            return new Response((int)restResponse.StatusCode, restResponse.Content);
        }

        private Method GetRestMethod(Request request)
        {
            switch (request.Method)
            {
                case RequestMethod.GET: return Method.GET;
                case RequestMethod.POST: return Method.POST;
                case RequestMethod.PUT: return Method.PUT;
                case RequestMethod.DELETE: return Method.DELETE;
                default: throw new InvalidOperationException();
            }
        }
    }
}
