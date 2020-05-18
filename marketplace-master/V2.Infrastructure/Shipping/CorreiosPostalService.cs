using Application.Config;
using Domain.DTO;
using Domain.Interfaces;
using Framework;
using Infra.Crosscutting.Correios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Shipping
{
    public class CorreiosPostalService : IPostalService
    {
        public OperationResult<decimal> GetShippingCost(ParcelInfo parcelInfo)
        {
            var client = new CorreiosClient();
            var request = CreateRequest(parcelInfo);
            var response = client.CalPrecoPrazo(request);

            return ParseResponse(response);
        }

        public async Task<OperationResult<decimal>> GetShippingCostAsync(ParcelInfo parcelInfo)
        {
            var client = new CorreiosClient();
            var request = CreateRequest(parcelInfo);
            var response = await client.CalPrecoPrazoAsync(request);

            return ParseResponse(response);
        }

        private ICalcPrecoPrazoRequest CreateRequest(ParcelInfo parcelInfo)
        {
            var request = new CalcPrecoPrazoRequest
            {
                Altura = parcelInfo.Height,
                Comprimento = parcelInfo.Length,
                Largura = parcelInfo.Width,
                Peso = parcelInfo.Weight,

                CepOrigem = parcelInfo.ZipCodeFrom,
                CepDestino = parcelInfo.ZipCodeTo,

                Formato = FormatoEncomenda.CaixaPacote
            };

            if (MaaSConfig.Current.Product.CorreiosContract.UseContractCode)
            {
                var config = MaaSConfig.Current.Product.CorreiosContract;
                request.CodigoEmpresa = config.Code;
                request.SenhaEmpresa = config.Password;

                switch (parcelInfo.DeliveryMode)
                {
                    case ParcelDeliveryMode.Normal:
                        request.ServicosContratoEmpresa = new string[] { config.PacServiceCode };
                        break;
                    case ParcelDeliveryMode.Express:
                        request.ServicosContratoEmpresa = new string[] { config.SedexServiceCode };
                        break;
                    default:
                        throw new InvalidOperationException($"Delivery mode '{parcelInfo.DeliveryMode}' not supported.");
                }
            }
            else
            {
                switch (parcelInfo.DeliveryMode)
                {
                    case ParcelDeliveryMode.Normal:
                        request.Servicos = ServicosEntrega.PacVarejo;
                        break;
                    case ParcelDeliveryMode.Express:
                        request.Servicos = ServicosEntrega.SedexVarejo;
                        break;
                    default:
                        throw new InvalidOperationException($"Delivery mode '{parcelInfo.DeliveryMode}' not supported.");
                }
            }

            return request;
        }

        private OperationResult<decimal> ParseResponse(ICalcPrecoPrazoResponse response)
        {
            var eligible = response.Servicos
                .Where(x => x.CodigoRetornado == "0" && x.MensagemRetornada == null)
                .FirstOrDefault();

            return eligible == null
                ? new ErrorDictionary("", "No results.")
                : new OperationResult<decimal>(eligible.Valor);
        }
    }
}
