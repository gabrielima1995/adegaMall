using System.ComponentModel.DataAnnotations;

namespace PaymentIntegrator.Core.Common
{
    /// <summary>
    /// Tipo de conta bancária.
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// Conta corrente.
        /// </summary>
        [Display(Name = "Conta corrente")]
        ContaCorrente,

        /// <summary>
        /// Conta poupança.
        /// </summary>
        [Display(Name = "Poupança")]
        Poupanca
    }
}