using Framework;
using System;
using System.IO;
using System.Net;

namespace Infra.Crosscutting.ExchangeRate.Providers
{
    public class ProMastersProvider : IExchangeRateProvider
    {
        private const string EndpointTemplate = "http://api.promasters.net.br/cotacao/v1/valores?moedas={0}&alt=json";

        private readonly string _currencyCode;
        private readonly string _endpoint;

        public ProMastersProvider(string currencyCode)
        {
            _currencyCode = currencyCode;
            _endpoint = string.Format(EndpointTemplate, currencyCode);
        }

        public OperationResult<decimal> GetRate()
        {
            var errors = new ErrorDictionary();

            try
            {
                var request = WebRequest.CreateHttp(string.Format(_endpoint));

                request.Accept = "application/json";
                request.UserAgent = "SubCategorys Met v1";

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var sr = new StreamReader(response.GetResponseStream()))
                        {
                            var content = sr.ReadToEnd().FromJson<dynamic>();

                            if (content.valores != null)
                            {
                                var currency = content.valores[_currencyCode.ToUpper()];

                                if (currency != null)
                                {
                                    return new OperationResult<decimal>(Convert.ToDecimal(currency["valor"]));
                                }

                                errors.Add("ProMastersProvider", "Currency not found.");
                            }

                            errors.Add("ProMastersProvider", "Valores not found.");
                        }
                    }
                    else
                    {
                        errors.Add("ProMastersProvider", "Status code: " + response.StatusCode);
                    }
                }
            }
            catch (WebException ex)
            {
                errors.Add("ProMastersProvider", "Status code: " + ex.Status);
            }

            return new OperationResult<decimal>(errors);
        }
    }
}