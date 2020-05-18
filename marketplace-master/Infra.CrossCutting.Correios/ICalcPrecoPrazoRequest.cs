using System.Collections.Generic;

namespace Infra.Crosscutting.Correios
{
    public interface ICalcPrecoPrazoRequest
    {
        /// <summary>
        /// Seu código administrativo junto à ECT. O código está disponível no corpo do contrato firmado com os Correios
        /// </summary>
        string CodigoEmpresa { get; set; }

        /// <summary>
        /// Senha para acesso ao serviço, associada ao seu código administrativo. A senha inicial corresponde aos 8 primeiros dígitos do CNPJ informado no contrato.
        /// </summary>
        string SenhaEmpresa { get; set; }

        /// <summary>
        /// Código do(s) serviço(s) padrão. Use somente se Código e Senha do contrato da empresa não forem especificados. Do contrário, use <see cref="ServicosContratoEmpresa"/>.
        /// </summary>
        ServicosEntrega Servicos { get; set; }

        /// <summary>
        /// Código do(s) serviço(s) definidos no contrato da empresa. Necessário fornecer também Código e Senha do contrato da empresa. Do contrário, use <see cref="Servicos"/>.
        /// </summary>
        IEnumerable<string> ServicosContratoEmpresa { get; set; }

        /// <summary>
        /// CEP de origem.
        /// </summary>
        string CepOrigem { get; set; }

        /// <summary>
        /// CEP de destino.
        /// </summary>
        string CepDestino { get; set; }

        /// <summary>
        /// Peso da encomenda, incluindo sua embalagem. O peso deve ser informado em quilogramas. Se o formato for Envelope, o valor máximo permitido será 1 kg.
        /// </summary>
        decimal Peso { get; set; }

        /// <summary>
        /// Formato da encomenda (incluindo embalagem).
        /// </summary>
        FormatoEncomenda Formato { get; set; }

        /// <summary>
        /// Comprimento da encomenda (incluindo embalagem), em centímetros.
        /// </summary>
        decimal Comprimento { get; set; }

        /// <summary>
        /// Altura da encomenda (incluindo embalagem), em centímetros.Se o formato for envelope, informar zero (0).
        /// </summary>
        decimal Altura { get; set; }

        /// <summary>
        /// Largura da encomenda (incluindo embalagem), em centímetros.
        /// </summary>
        decimal Largura { get; set; }

        /// <summary>
        /// Diâmetro da encomenda (incluindo embalagem), em centímetros.
        /// </summary>
        decimal Diametro { get; set; }

        /// <summary>
        /// Indica se a encomenda será entregue com o serviço adicional mão própria.
        /// </summary>
        bool MaoPropria { get; set; }

        /// <summary>
        /// Indica se a encomenda será entregue com o serviço adicional valor declarado. Neste campo deve ser apresentado o valor declarado desejado, em Reais.
        /// </summary>
        decimal? ValorDeclarado { get; set; }

        /// <summary>
        /// Indica se a encomenda será entregue com o serviço adicional aviso de recebimento.
        /// </summary>
        bool AvisoRecebimento { get; set; }

        bool IsValid(out string[] errors);
    }
}