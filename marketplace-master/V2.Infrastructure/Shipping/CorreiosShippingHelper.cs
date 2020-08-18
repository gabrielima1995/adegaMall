using Application.Config;
using Domain.Entities;
using Framework;
using Infra.Crosscutting.Correios;
using Infra.Crosscutting.Globalization;
using System.Collections.Generic;

namespace V2.Infrastructure.Shipping
{
    internal class CorreiosShippingHelper
    {
        public OperationResult<CorreioShippingResult> CalculatePACShipping(IEnumerable<ItemFrete> items, Address sellerAddress, Address shippingAddress)
        {
            return CalculateShipping(false, items, sellerAddress, shippingAddress);
        }

        public OperationResult<CorreioShippingResult> CalculateSedexShipping(IEnumerable<ItemFrete> items, Address sellerAddress, Address shippingAddress)
        {
            return CalculateShipping(true, items, sellerAddress, shippingAddress);
        }

        private CorreioShippingResult CalculateShipping(bool sedex, IEnumerable<ItemFrete> items, Address sellerAddress, Address shippingAddress)
        {
            var pacote = new PacoteFrete(items);
            var correiosClient = new CorreiosClient();

            var request = new CalcPrecoPrazoRequest
            {
                Servicos = sedex ? ServicosEntrega.SedexVarejo : ServicosEntrega.PacVarejo,
                CepOrigem = sellerAddress.ZipCode,
                CepDestino = shippingAddress.ZipCode,
                Peso = (decimal)pacote.Peso,
                Formato = FormatoEncomenda.CaixaPacote,
                Comprimento = (decimal)pacote.Comprimento,
                Altura = (decimal)pacote.Altura,
                Largura = (decimal)pacote.Largura,
                MaoPropria = false,
                AvisoRecebimento = false
            };

            if (MaaSConfig.Current.Product.CorreiosContract.UseContractCode)
            {
                var config = MaaSConfig.Current.Product.CorreiosContract;
                request.CodigoEmpresa = config.Code;
                request.SenhaEmpresa = config.Password;
                request.ServicosContratoEmpresa = sedex ? new string[] { config.SedexServiceCode } : new string[] { config.PacServiceCode };
            }
            else request.Servicos = sedex ? ServicosEntrega.SedexVarejo : ServicosEntrega.PacVarejo;

            var response = correiosClient.CalPrecoPrazo(request);
            return new CorreioShippingResult(GlobalizationHelper.GetString(response.Servicos[0].MensagemRetornada), response.Servicos[0].CodigoRetornado, response.Servicos[0].Valor, response.Servicos[0].PrazoEntrega);
        }
    }
}