using Framework.Json;
using HttpClientFactoryLite;
using Infra.Crosscutting.AsapLog.ConsultaFrete;

namespace Infra.Crosscutting.AsapLog
{
    public class AsapLogApiClient
    {
        public AsapLogConsultaFreteResource ConsultaFrete { get; private set; }

        public AsapLogApiClient(
            IHttpClientFactory httpClientFactory,
            IJsonProvider jsonProvider)
        {
            var httpClient = httpClientFactory.CreateClient("asaplog");

            ConsultaFrete = new AsapLogConsultaFreteResource(httpClient, jsonProvider);
        }
    }
}