using System.Threading.Tasks;
using Framework.ApiProviders;
using Framework.AsapLog;
using Framework.AsapLog.ConsultaFrete;

namespace Infra.Crosscutting.AsapLog.ConsultaFrete
{
    public class AsapLogConsultaFreteProvider : IAsapLogConsultaFreteProvider
    {
        private readonly AsapLogApiClient _client;

        public AsapLogConsultaFreteProvider(AsapLogApiClient client)
        {
            _client = client;
        }

        public async Task<ApiResponse<ConsultaFreteResponse>> ConsultaFreteAsync(AsapLogCredentials credentials, ConsultaFreteRequest command)
        {
            return await _client.ConsultaFrete.ConsultaFreteAsync(credentials, command)
                .ConfigureAwait(false);
        }
    }
}