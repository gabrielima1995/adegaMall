using Newtonsoft.Json;

namespace PaymentIntegrator.Iugu.Suppliers
{
    public class SubAccountResponse
    {
        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        public string Name { get; set; }

        [JsonProperty("live_api_token")]
        public string LiveApiToken { get; set; }

        [JsonProperty("test_api_token")]
        public string TestApiToken { get; set; }

        [JsonProperty("user_token")]
        public string UserToken { get; set; }
    }
}