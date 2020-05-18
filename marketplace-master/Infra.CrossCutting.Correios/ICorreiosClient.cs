using System.Threading.Tasks;

namespace Infra.Crosscutting.Correios
{
    public interface ICorreiosClient
    {
        Task<ICalcPrecoPrazoResponse> CalPrecoPrazoAsync(ICalcPrecoPrazoRequest request);

        ICalcPrecoPrazoResponse CalPrecoPrazo(ICalcPrecoPrazoRequest request);
    }
}