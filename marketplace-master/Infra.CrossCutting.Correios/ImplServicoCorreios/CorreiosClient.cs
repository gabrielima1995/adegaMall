using Application.Config;
using Infra.Crosscutting.Correios.Infra.CrossCutting.Correios.External;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog.Core;

namespace Infra.Crosscutting.Correios
{
    public class CorreiosClient : ICorreiosClient
    {
        private readonly CalcPrecoPrazoWSSoapClient soapClient;
        private Logger _logger = CorreiosLogger.Instance;

        public CorreiosClient()
        {
            var timeout = TimeSpan.FromSeconds(MaaSConfig.Current.Product.CorreiosTimeoutSeconds);
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.None)
            {
                OpenTimeout = timeout,
                CloseTimeout = timeout,
                SendTimeout = timeout,
                ReceiveTimeout = timeout
            };

            soapClient = new CalcPrecoPrazoWSSoapClient(binding, new EndpointAddress("http://ws.correios.com.br/calculador/CalcPrecoPrazo.asmx"));
        }

        public async Task<ICalcPrecoPrazoResponse> CalPrecoPrazoAsync(ICalcPrecoPrazoRequest request)
        {
            ValidateRequest(request);
            var serviceParams = new CalcPrecoPrazoParams(request);
            CalcPrecoPrazoResponse result = null;

            var stopWatch = new Stopwatch();

            try
            {
                stopWatch.Start();

                var serviceResult = await soapClient.CalcPrecoPrazoAsync(
                    serviceParams.nCdEmpresa,
                    serviceParams.sDsSenha,
                    serviceParams.nCdServico,
                    serviceParams.sCepOrigem,
                    serviceParams.sCepDestino,
                    serviceParams.nVlPeso,
                    serviceParams.nCdFormato,
                    serviceParams.nVlComprimento,
                    serviceParams.nVlAltura,
                    serviceParams.nVlLargura,
                    serviceParams.nVlDiametro,
                    serviceParams.sCdMaoPropria,
                    serviceParams.nVlValorDeclarado,
                    serviceParams.sCdAvisoRecebimento);

                result = new CalcPrecoPrazoResponse(serviceResult);
            }
            catch (Exception ex)
            {
                if (ex is TimeoutException || ex is CommunicationException)
                    result = CalcPrecoPrazoResponse.CommunicationError(request.Servicos, "SHIPPING_CORREIOS_ERROR_MESSAGE");
                else
                    throw;
            }
            finally
            {
                stopWatch.Stop();

                _logger.Information($"Request completed in {stopWatch.ElapsedMilliseconds} ms");
                _logger.Information($"Request: {JsonConvert.SerializeObject(serviceParams)}");
                _logger.Information("Response: " + (result == null ? "(null)" : JsonConvert.SerializeObject(result)));
            }
            
            return result;
        }

        public ICalcPrecoPrazoResponse CalPrecoPrazo(ICalcPrecoPrazoRequest request)
        {
            ValidateRequest(request);
            var serviceParams = new CalcPrecoPrazoParams(request);
            CalcPrecoPrazoResponse result = null;

            var stopWatch = new Stopwatch();

            try
            {
                stopWatch.Start();

                var serviceResult = soapClient.CalcPrecoPrazo(
                    serviceParams.nCdEmpresa,
                    serviceParams.sDsSenha,
                    serviceParams.nCdServico,
                    serviceParams.sCepOrigem,
                    serviceParams.sCepDestino,
                    serviceParams.nVlPeso,
                    serviceParams.nCdFormato,
                    serviceParams.nVlComprimento,
                    serviceParams.nVlAltura,
                    serviceParams.nVlLargura,
                    serviceParams.nVlDiametro,
                    serviceParams.sCdMaoPropria,
                    serviceParams.nVlValorDeclarado,
                    serviceParams.sCdAvisoRecebimento);

                result = new CalcPrecoPrazoResponse(serviceResult);
            }
            catch (Exception ex)
            {
                if (ex is TimeoutException || ex is CommunicationException)
                    result = CalcPrecoPrazoResponse.CommunicationError(request.Servicos, "SHIPPING_CORREIOS_ERROR_MESSAGE");
                else
                    throw;
            }
            finally
            {
                stopWatch.Stop();

                _logger.Information($"Request completed in {stopWatch.ElapsedMilliseconds} ms");
                _logger.Information($"Request: {JsonConvert.SerializeObject(serviceParams)}");
                _logger.Information("Response: " + (result == null ? "(null)" : JsonConvert.SerializeObject(result)));
            }

            return result;
        }

        private void ValidateRequest(ICalcPrecoPrazoRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (!request.IsValid(out string[] validationErrors))
            {
                var sb = new StringBuilder();
                foreach (var err in validationErrors) sb.AppendLine(err);
                throw new ArgumentException("Parâmetros inválidos para cáculo de preço e prazo de frete pelos Correios: " + sb.ToString());
            }
        }
    }
}