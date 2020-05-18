using Application.Config;
using Domain.Entities;
using Framework;
using Infra.Crosscutting.Correios;
using Infra.Crosscutting.Globalization;
using System;
using System.Collections.Generic;

namespace V2.Infrastructure.Shipping
{
    internal class CorreiosShippingHelper
    {
        public OperationResult<CorreioShippingResult> CalculatePACShipping(IEnumerable<CorreiosItemEnvio> items, Address sellerAddress, Address shippingAddress)
        {
            return CalculateShipping(false, items, sellerAddress, shippingAddress);
        }

        public OperationResult<CorreioShippingResult> CalculateSedexShipping(IEnumerable<CorreiosItemEnvio> items, Address sellerAddress, Address shippingAddress)
        {
            return CalculateShipping(true, items, sellerAddress, shippingAddress);
        }

        private CorreioShippingResult CalculateShipping(bool sedex, IEnumerable<CorreiosItemEnvio> items, Address sellerAddress, Address shippingAddress)
        {
            var pacote = new Pacote(items);
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

    internal class Pacote
    {
        private const double MinComprimento = 16D;
        private const double MinLargura = 11D;
        private const double MinAltura = 2D;

        public List<CorreiosItemEnvio> Itens { get; set; } = new List<CorreiosItemEnvio>();

        public double Peso { get; set; }

        public double Comprimento { get; set; }

        public double Largura { get; set; }

        public double Altura { get; set; }

        public double Volume { get; set; }

        public Pacote() { }

        public Pacote(IEnumerable<CorreiosItemEnvio> itens)
        {
            foreach (var item in itens)
                AdicionarItem(item);
        }

        public void AdicionarItem(CorreiosItemEnvio item)
        {
            Itens.Add(item);
            CalcularMedidas(item);
        }

        private void CalcularMedidas(CorreiosItemEnvio item)
        {
            Peso += item.Peso;
            Volume += item.Volume;

            if (Itens.Count == 1)
            {
                Comprimento = item.Comprimento;
                Largura = item.Largura;
                Altura = item.Altura;
            }
            else
            {
                var dimensao = Math.Pow(Volume, (1D / 3));
                Comprimento = Largura = Altura = dimensao;
            }

            GarantirMedidasMinimas();
        }

        private void GarantirMedidasMinimas()
        {
            Comprimento = Math.Max(Comprimento, MinComprimento);
            Largura = Math.Max(Largura, MinLargura);
            Altura = Math.Max(Altura, MinAltura);
        }
    }

    public class CorreiosItemEnvio
    {
        public double Peso { get; set; }

        public double Comprimento { get; set; }

        public double Largura { get; set; }

        public double Altura { get; set; }

        public double Volume => Comprimento * Altura * Largura;

        public CorreiosItemEnvio() { }

        public CorreiosItemEnvio(double peso, double comprimento, double largura, double altura)
        {
            Peso = peso;
            Comprimento = comprimento;
            Largura = largura;
            Altura = altura;
        }
    }
}