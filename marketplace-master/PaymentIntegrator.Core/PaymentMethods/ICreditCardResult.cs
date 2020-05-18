namespace PaymentIntegrator.Core.PaymentMethods
{
    /// <summary>
    /// Representa um cartão de crédito salvo no gateway de pagamento.
    /// </summary>
    public interface ICreditCardResult
    {
        /// <summary>
        /// Identificador do cartão de crédito no gateway de pagamento.
        /// </summary>
        string ReferenceId { get; }

        /// <summary>
        /// Token do cartão de crédito, gerado pelo gateway de pagamento.
        /// </summary>
        string Token { get; }

        /// <summary>
        /// Número do cartão de crédito para exibição.
        /// </summary>
        string DisplayNumber { get; }

        /// <summary>
        /// Bandeira do cartão de crédito.
        /// </summary>
        string Brand { get; }

        /// <summary>
        /// Nome do portador do cartão de crédito.
        /// </summary>
        string HolderName { get; }
    }
}