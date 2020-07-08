using Application.Config;
using Framework.ApiProviders;
using Framework.AsapLog;
using Framework.AsapLog.ConsultaFrete;
using Framework.Json;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infra.Crosscutting.AsapLog.ConsultaFrete
{
    public class AsapLogConsultaFreteResource : AsapLogApiResourceBase
    {
        private readonly MaaSConfig _config = MaaSConfig.Current;
        private readonly CultureInfo _ciUs = CultureInfo.GetCultureInfo("en-us");

        public AsapLogConsultaFreteResource(
            HttpClient client,
            IJsonProvider jsonProvider)
            : base(client, jsonProvider)
        {
        }

        public async Task<ApiResponse<ConsultaFreteResponse>> ConsultaFreteAsync(AsapLogCredentials credentials, ConsultaFreteRequest command)
        {
            var endpointSettings = _config.Product.AsapLogShipping.ConsultaFrete;
            var peso = command.Peso.DisplayDecimal(1, _ciUs);
            var valor = command.Valor.DisplayDecimal(2, _ciUs);
            var cep = command.Cep.Replace(".", "").Replace("-", "");
            var altura = command.Altura;
            var largura = command.Largura;
            var profundidade = command.Profundidade;

            var uri = $"{endpointSettings.Url}?peso={peso}&valor={valor}&cep={cep}&altura={altura}&largura={largura}&profundidade={profundidade}";

            HttpRequestMessage CreateRequest()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.Authorization = GetCredentials(credentials);
                return request;
            }

            return await SendAsync<ConsultaFreteResponse>(endpointSettings, CreateRequest)
                .ConfigureAwait(false);
        }
    }
}