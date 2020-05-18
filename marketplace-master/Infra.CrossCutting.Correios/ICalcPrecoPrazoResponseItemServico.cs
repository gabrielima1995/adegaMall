namespace Infra.Crosscutting.Correios
{
    public interface ICalcPrecoPrazoResponseItemServico
    {
        /// <summary>
        /// Código do Serviço de Entrega.
        /// </summary>
        ServicosEntrega CodigoServicoEntrega { get; set; }

        /// <summary>
        /// Preço total da encomenda, em Reais, incluindo os preços dos serviços opcionais.
        /// </summary>
        decimal Valor { get; set; }

        /// <summary>
        /// Prazo estimado em dias para entrega do produto. Se o valor retornado for 0 (zero), indica que o prazo não foi retornado corretamente.
        /// </summary>
        int PrazoEntrega { get; set; }

        /// <summary>
        /// Preço do serviço adicional Mão Própria.
        /// </summary>
        decimal ValorMaoPropria { get; set; }

        /// <summary>
        /// Preço do serviço adicional Aviso de Recebimento.
        /// </summary>
        decimal ValorAvisoRecebimento { get; set; }

        /// <summary>
        /// Preço do serviço adicional Valor Declarado.
        /// </summary>
        decimal ValorDeclarado { get; set; }

        /// <summary>
        /// Informa se a localidade informada possui entrega domiciliária. Se o prazo não for retornado corretamente, o retorno deste parâmetro será vazio.
        /// </summary>
        bool PermiteEntregaDomiciliar { get; set; }

        /// <summary>
        /// Informa se a localidade informada possui entrega domiciliária aos sábados. Se o prazo não for retornado corretamente, o retorno deste parâmetro será vazio.
        /// </summary>
        bool PermiteEntregaSabado { get; set; }

        /// <summary>
        /// Códigos de retorno do correios. Podendo ser um erro ou um aviso que a entraga não será da forma tradicional (entrgue no prazo ou no endereço indicado).
        /// </summary>
        string CodigoRetornado { get; set; }

        /// <summary>
        /// Retorna uma menssagem sobre: a possibilidade, impossbilidade ou adversidade na entrega do produto.
        /// </summary>
        string MensagemRetornada { get; set; }
    }
}