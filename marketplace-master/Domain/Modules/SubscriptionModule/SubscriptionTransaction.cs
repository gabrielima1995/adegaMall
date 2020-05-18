using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.SubscriptionModule
{
    /// <summary>
    /// Transação de pagamento/cancelamento de uma assinatura.
    /// </summary>
    public class SubscriptionTransaction
    {
        /// <summary>
        /// Identificador da transação.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador da transação no gateway de pagamento.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Status da transação.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Data e hora da transação.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Conteúdo da transação, vindo do gateway de pagamento.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Identificador da assinatura no gateway de pagamento, relacionada à transação.
        /// </summary>
        public string SubsReferenceId { get; set; }

        [Required]
        public Subscription Subscription { get; set; }

        [ForeignKey("Subscription")]
        public string Subscription_SellerId { get; set; }

        public SubscriptionTransaction()
        {
            Id = Guid.NewGuid();
            Date = DateTime.UtcNow;
        }
    }
}