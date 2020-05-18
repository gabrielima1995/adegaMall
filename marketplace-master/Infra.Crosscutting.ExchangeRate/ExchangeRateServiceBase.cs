using Framework;
using Infra.Crosscutting.ExchangeRate.Providers;
using System;
using System.Collections.Generic;

namespace Infra.Crosscutting.ExchangeRate
{
    public abstract class ExchangeRateServiceBase : IExchangeRateService
    {
        private readonly IList<IExchangeRateProvider> _providers;

        protected ExchangeRateServiceBase(IList<IExchangeRateProvider> providers)
        {
            if (providers == null) throw new ArgumentNullException(nameof(providers));

            _providers = providers;
        }

        public OperationResult<decimal> GetRate()
        {
            OperationResult<decimal> validation = null;

            foreach (var provider in _providers)
            {
                validation = provider.GetRate();

                if (validation.OK)
                {
                    break;
                }
            }

            return validation;
        }
    }
}