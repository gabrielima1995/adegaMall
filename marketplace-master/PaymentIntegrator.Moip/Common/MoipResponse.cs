using RestSharp;
using System;
using System.Linq;
using System.Net;

namespace PaymentIntegrator.Moip.Common
{
    public abstract class MoipResponse
    {
        public MoipResponse(IRestResponse restResponse)
        {
            RestResponse = restResponse ?? throw new ArgumentNullException(nameof(restResponse));
        }

        public IRestResponse RestResponse { get; }

        public bool IsSuccessful => SuccessfulStatusCodes.Contains(RestResponse.StatusCode);

        public bool IsFailure => !IsSuccessful;

        protected abstract HttpStatusCode[] SuccessfulStatusCodes { get; }

        public string GetErrorString()
        {
            return $"{RestResponse.StatusCode} ({RestResponse.StatusDescription}): {RestResponse.Content}";
        }
    }
}
