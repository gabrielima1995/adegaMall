using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.ApiProviders;

namespace Framework.TotalExpress.ConsultaFrete
{
    public interface ITotalExpressConsultaFreteProvider
    {
        ApiResponse<ConsultaFreteResponse> ConsultaFreteAsync(TotalExpressCredentials credentials, ConsultaFreteRequest command);
    }
}
