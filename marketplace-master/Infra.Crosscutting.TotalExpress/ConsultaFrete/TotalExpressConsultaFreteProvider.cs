using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Framework.ApiProviders;
using Framework.TotalExpress;
using Framework.TotalExpress.ConsultaFrete;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Application.Config;
using Domain.Config.Modules.Product;

namespace Infra.Crosscutting.TotalExpress.ConsultaFrete
{
    public class TotalExpressConsultaFreteProvider : ITotalExpressConsultaFreteProvider
    {
        public TotalExpressConsultaFreteProvider()
        {
        }

        public ApiResponse<ConsultaFreteResponse> ConsultaFreteAsync(TotalExpressCredentials credentials, ConsultaFreteRequest dto)
        {
            try
            {
                var config = MaaSConfig.Current.Product.TotalExpressShipping;
                var request = CreateSoapWebRequest(credentials.Usuario, credentials.Senha);
                var soapRequestBody = new XmlDocument();
                soapRequestBody.LoadXml(MountSoapEnv(dto, config));

                using (var stream = request.GetRequestStream())
                {
                    soapRequestBody.Save(stream);
                }

                using (var service = request.GetResponse())
                {
                    using (var reader = new StreamReader(service.GetResponseStream()))
                    {
                        var xmlString = reader.ReadToEnd();
                        var xml = XElement.Parse(xmlString).Descendants("calcularFreteResponse").Descendants();

                        var codeProc = xml.FirstOrDefault(e => e.Name == "CodigoProc")?.Value;
                        var erroConsultaFrete = xml.FirstOrDefault(e => e.Name == "ErroConsultaFrete")?.Value;

                        if (erroConsultaFrete != null)
                        {
                            return new ApiResponse<ConsultaFreteResponse>(new ApiErrorMessage(erroConsultaFrete), 0);
                        }

                        return new ApiResponse<ConsultaFreteResponse>(new ConsultaFreteResponse
                        {
                            CodigoProc = int.Parse(codeProc),
                            Prazo = int.Parse(xml.FirstOrDefault(e => e.Name == "Prazo").Value),
                            Preco = decimal.Parse(xml.FirstOrDefault(e => e.Name == "ValorServico").Value, CultureInfo.GetCultureInfo("pt-br"))
                        },0);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConsultaFreteResponse>(new ApiErrorMessage(ex.Message), 0);
            }
        }

        private static HttpWebRequest CreateSoapWebRequest(string usuario, string senha)
        {
            var request = (HttpWebRequest)WebRequest.Create(@"https://edi.totalexpress.com.br/webservice_calculo_frete.php?wsdl");
            request.Headers.Add("SOAPAction: \"urn: simulaFrete#calcularFrete\"");
            request.ContentType = "text/xml;charset=\"ISO-8859-1\"";
            request.Accept = "text/xml";
            request.Method = "POST";

            var encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{usuario}:{senha}"));
            request.Headers.Add("Authorization", "Basic " + encoded);
            return request;
        }

        private static string MountSoapEnv(ConsultaFreteRequest dto, TotalExpressShippingConfig config)
        {
            var result = $@"<soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:calcularFrete"">
                   <soapenv:Header/>
                   <soapenv:Body>
                      <urn:calcularFrete soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
                         <calcularFreteRequest xsi:type=""web:calcularFreteRequest"" xmlns:web=""http://edi.totalexpress.com.br/soap/webservice_calculo_frete.total"">
                            <!--You may enter the following 9 items in any order-->
                            <TipoServico xsi:type=""xsd:string"">{config.TipoServico}</TipoServico>
                            <CepDestino xsi:type=""xsd:nonNegativeInteger"">{dto.Cep.ExtractDigits()}</CepDestino>
                            <Peso xsi:type=""xsd:string"">{dto.Peso.ToString("F1", CultureInfo.GetCultureInfo("pt-br"))}</Peso>
                            <ValorDeclarado xsi:type=""xsd:string"">{dto.Valor.ToString("F2", CultureInfo.GetCultureInfo("pt-br"))}</ValorDeclarado>
                            <TipoEntrega xsi:type=""xsd:nonNegativeInteger"">0</TipoEntrega>
                            <!--Optional:-->
                            <ServicoCOD xsi:type=""xsd:boolean"">0</ServicoCOD>
                            <!--Optional:-->
                            <Altura xsi:type=""xsd:nonNegativeInteger"">{dto.Altura}</Altura>
                            <!--Optional:-->
                            <Largura xsi:type=""xsd:nonNegativeInteger"">{dto.Largura}</Largura>
                            <!--Optional:-->
                            <Profundidade xsi:type=""xsd:nonNegativeInteger"">{dto.Profundidade}</Profundidade>
                         </calcularFreteRequest>
                      </urn:calcularFrete>
                   </soapenv:Body>
                </soapenv:Envelope>";

            return result;
        }
    }
}