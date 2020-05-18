using Framework;

namespace Infra.Crosscutting.ExchangeRate.Providers
{
    public interface IExchangeRateProvider
    {
        OperationResult<decimal> GetRate();
    }
}