using System;

namespace PaymentIntegrator.Core.PaymentMethods
{
    /// <summary>
    /// Representa um cartão de crédito.
    /// </summary>
    public interface ICreditCard
    {
        /// <summary>
        /// Número do cartão de crédito.
        /// </summary>
        string Number { get; }

        /// <summary>
        /// Código de segurança.
        /// </summary>
        string CVV { get; }

        /// <summary>
        /// Nome completo do portador.
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Documento de identificação do portador
        /// </summary>
        string DocumentNumber { get; }

        /// <summary>
        /// Data de nascimento do portador do cartão
        /// </summary>
        DateTime? DateOfBirth { get; }

        /// <summary>
        /// Mês da data de expiração do cartão de crédito.
        /// </summary>
        int Month { get; }

        /// <summary>
        /// Ano da data de expiração do cartão de crédito.
        /// </summary>
        int Year { get; }
    }
}