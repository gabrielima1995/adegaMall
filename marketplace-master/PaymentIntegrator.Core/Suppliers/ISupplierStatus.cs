namespace PaymentIntegrator.Core.Suppliers
{
    /// <summary>
    /// Representa a situação de um fornecedor/recebedor no gateway de pagamento.
    /// </summary>
    public interface ISupplierStatus
    {
        /// <summary>
        /// Indica se o fornecedor/recebedor está verificado no gateway de pagamento.
        /// </summary>
        bool IsVerified { get; }

        /// <summary>
        /// Situação do fornecedor/recebedor no gateway de pagamento.
        /// </summary>
        string Status { get; }

        /// <summary>
        /// Feedback do gateway de pagamento sobre o fornecedor/recebedor.
        /// </summary>
        string Feedback { get; }
    }
}