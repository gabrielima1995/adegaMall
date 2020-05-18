using Infra.Crosscutting.ExchangeRate.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Crosscutting.ExchangeRate.USD
{
    public class USDExchangeRateService : ExchangeRateServiceBase, IUSDExchangeRateService
    {
        public USDExchangeRateService()
            : base(new List<IExchangeRateProvider>
            {
                new ProMastersProvider("USD"),
                new EconomiaAwesomeApiProvider("USD")
            })
        {
        }
    }
}