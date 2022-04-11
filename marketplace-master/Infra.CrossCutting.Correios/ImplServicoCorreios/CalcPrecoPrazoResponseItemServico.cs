using Infra.Crosscutting.Correios.Infra.CrossCutting.Correios.External;
using System;
using System.Globalization;

namespace Infra.Crosscutting.Correios
{
    public class CalcPrecoPrazoResponseItemServico : ICalcPrecoPrazoResponseItemServico
    {
        /// <summary>
        /// Código do Serviço de Entrega.
        /// </summary>
        public ServicosEntrega CodigoServicoEntrega { get; set; }

        /// <summary>
        /// Preço total da encomenda, em Reais, incluindo os preços dos serviços opcionais.
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Prazo estimado em dias para entrega do produto. Se o valor retornado for 0 (zero), indica que o prazo não foi retornado corretamente.
        /// </summary>
        public int PrazoEntrega { get; set; }

        /// <summary>
        /// Preço do serviço adicional Mão Própria.
        /// </summary>
        public decimal ValorMaoPropria { get; set; }

        /// <summary>
        /// Preço do serviço adicional Aviso de Recebimento.
        /// </summary>
        public decimal ValorAvisoRecebimento { get; set; }

        /// <summary>
        /// Preço do serviço adicional Valor Declarado.
        /// </summary>
        public decimal ValorDeclarado { get; set; }

        /// <summary>
        /// Informa se a localidade informada possui entrega domiciliária. Se o prazo não for retornado corretamente, o retorno deste parâmetro será vazio.
        /// </summary>
        public bool PermiteEntregaDomiciliar { get; set; }

        /// <summary>
        /// Informa se a localidade informada possui entrega domiciliária aos sábados. Se o prazo não for retornado corretamente, o retorno deste parâmetro será vazio.
        /// </summary>
        public bool PermiteEntregaSabado { get; set; }


        /// <summary>
        /// Códigos de retorno do correios. Podendo ser um erro ou um aviso que a entraga não será da forma tradicional (entrgue no prazo ou no endereço indicado).
        /// </summary>
        public string CodigoRetornado { get; set; }

        /// <summary>
        /// Retorna uma menssagem sobre: a possibilidade, impossbilidade ou adversidade na entrega do produto.
        /// </summary>
        public string MensagemRetornada { get; set; }

        public string MensagemErroProvedor { get; set; }

        public CalcPrecoPrazoResponseItemServico()
        {
        }

        internal CalcPrecoPrazoResponseItemServico(cServico servico)
        {
            var ptBr = new CultureInfo("pt-BR");

            CodigoServicoEntrega = (ServicosEntrega)servico.Codigo;

            Valor = decimal.Parse(servico.Valor, ptBr);

            PrazoEntrega = int.Parse(servico.PrazoEntrega);

            ValorMaoPropria = decimal.Parse(servico.ValorMaoPropria, ptBr);

            ValorAvisoRecebimento = decimal.Parse(servico.ValorAvisoRecebimento, ptBr);

            PermiteEntregaDomiciliar = servico.EntregaDomiciliar.Equals("s", StringComparison.InvariantCultureIgnoreCase);

            PermiteEntregaSabado = servico.EntregaSabado.Equals("s", StringComparison.InvariantCultureIgnoreCase);

            CodigoRetornado = string.IsNullOrEmpty(servico.Erro) ? "0" : servico.Erro;

            MensagemRetornada = MensagemErroProvedor = servico.MsgErro;
        }

        internal CalcPrecoPrazoResponseItemServico(ServicosEntrega services, 
            string errorCode, string errorMessage, string mensagemErroProvedor)
        {
            CodigoServicoEntrega = services;
            CodigoRetornado = errorCode;
            MensagemRetornada = errorMessage;
            MensagemErroProvedor = mensagemErroProvedor;
        }
    }
}
