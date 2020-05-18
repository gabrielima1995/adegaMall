using Framework;
using System;
using System.IO;
using System.Net;

namespace Infra.Crosscutting.ExchangeRate.Providers
{
    public class EconomiaAwesomeApiProvider : IExchangeRateProvider
    {
        private const string EndpointTemplate = "https://economia.awesomeapi.com.br/json/{0}-BRL/1";

        private readonly string _endpoint;

        public EconomiaAwesomeApiProvider(string currencyCode)
        {
            _endpoint = string.Format(EndpointTemplate, currencyCode);
        }

        public OperationResult<decimal> GetRate()
        {
            var errors = new ErrorDictionary();

            try
            {
                var request = WebRequest.CreateHttp(string.Format(_endpoint));

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var sr = new StreamReader(response.GetResponseStream()))
                        {
                            var content = sr.ReadToEnd().FromJson<dynamic>();

                            if (content[0] != null)
                            {
                                var currency = content[0]["ask"];

                                if (currency != null)
                                {
                                    return new OperationResult<decimal>(Convert.ToDecimal(currency));
                                }

                                errors.Add("EconomiaAwesomeApiProvider", "Currency not found.");
                            }

                            errors.Add("EconomiaAwesomeApiProvider", "Valores not found.");
                        }
                    }
                    else
                    {
                        errors.Add("EconomiaAwesomeApiProvider", "Status code: " + response.StatusCode);
                    }
                }
            }
            catch (WebException ex)
            {
                errors.Add("EconomiaAwesomeApiProvider", "Status code: " + ex.Status);
            }

            return new OperationResult<decimal>(errors);
        }
    }
}