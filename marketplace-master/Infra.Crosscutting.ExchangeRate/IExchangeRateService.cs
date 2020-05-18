using Framework;

namespace Infra.Crosscutting.ExchangeRate
{
    public interface IExchangeRateService
    {
        OperationResult<decimal> GetRate();
    }
}