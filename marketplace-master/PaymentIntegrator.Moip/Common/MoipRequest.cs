using RestSharp;

namespace PaymentIntegrator.Moip.Common
{
    public class MoipRequest : RestRequest
    {
        public MoipRequest(string resource, Method method) : base(resource, method)
        {
        }
    }
}
