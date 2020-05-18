namespace PaymentIntegrator.Core.Plans
{
    /// <summary>
    /// Representa um plano de assinatura.
    /// </summary>
    public interface IPlan
    {
        /// <summary>
        /// Nome do plano.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Preço do plano.
        /// </summary>
        decimal Amount { get; }

        /// <summary>
        /// Frequência de cobrança, em dias.
        /// </summary>
        int Days { get; }

        /// <summary>
        /// Tempo de assinatura trial para o usuário, em dias.
        /// </summary>
        int TrialDays { get; }

        /// <summary>
        /// Identificador do plano no gateway de pagamento.
        /// </summary>
        string GatewayId { get; }
    }
}