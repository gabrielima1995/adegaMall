using PaymentIntegrator.Core.Common;

namespace PaymentIntegrator.Core.Suppliers
{
    /// <summary>
    /// Representa uma conta bancária de fornecedor.
    /// </summary>
    public interface IBankAccount
    {
        /// <summary>
        /// Entidade bancária.
        /// </summary>
        Bank Bank { get; }

        /// <summary>
        /// Tipo da conta bancária.
        /// </summary>
        AccountType AccountType { get; }

        /// <summary>
        /// Agência da conta bancária.
        /// </summary>
        string Branch { get; }

        /// <summary>
        /// Número da conta bancária.
        /// </summary>
        string Number { get; }
    }
}