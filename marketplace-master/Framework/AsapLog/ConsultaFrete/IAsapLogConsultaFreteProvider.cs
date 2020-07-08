using System.Threading.Tasks;
using Framework.ApiProviders;

namespace Framework.AsapLog.ConsultaFrete
{
    public interface IAsapLogConsultaFreteProvider
    {
        Task<ApiResponse<ConsultaFreteResponse>> ConsultaFreteAsync(AsapLogCredentials credentials, ConsultaFreteRequest command);
    }
}