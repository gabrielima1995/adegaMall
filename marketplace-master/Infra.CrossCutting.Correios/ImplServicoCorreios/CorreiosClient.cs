using Application.Config;
using Infra.Crosscutting.Correios.Infra.CrossCutting.Correios.External;
using System;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Crosscutting.Correios
{
    public class CorreiosClient : ICorreiosClient
    {
        private readonly CalcPrecoPrazoWSSoapClient soapClient;

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

            try
            {
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

                return new CalcPrecoPrazoResponse(serviceResult);
            }
            catch (Exception ex)
            {
                if (ex is TimeoutException || ex is CommunicationException)
                    return CalcPrecoPrazoResponse.CommunicationError(request.Servicos, "SHIPPING_CORREIOS_ERROR_MESSAGE");
                throw;
            }
        }

        public ICalcPrecoPrazoResponse CalPrecoPrazo(ICalcPrecoPrazoRequest request)
        {
            ValidateRequest(request);
            var serviceParams = new CalcPrecoPrazoParams(request);

            try
            {
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

                return new CalcPrecoPrazoResponse(serviceResult);
            }
            catch (Exception ex)
            {
                if (ex is TimeoutException || ex is CommunicationException)
                    return CalcPrecoPrazoResponse.CommunicationError(request.Servicos, "SHIPPING_CORREIOS_ERROR_MESSAGE");
                throw;
            }
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