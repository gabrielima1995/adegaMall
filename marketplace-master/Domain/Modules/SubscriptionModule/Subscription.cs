using Domain.Events;
using Domain.Events.SellerProfileUpdated;
using Domain.Events.Subscriptions;
using Framework;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Core.Plans;
using PaymentIntegrator.Core.Subscriptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.SubscriptionModule
{
    /// <summary>
    /// Assinatura de produto/serviço no marketplace.
    /// </summary>
    public class Subscription : ISubscription, IEntity
    {
        /// <summary>
        /// Identificador do usuário pertencente à assinatura.
        /// </summary>
        [Key]
        [ForeignKey("Seller")]
        public string SellerId { get; set; }

        /// <summary>
        /// Usuário pertercente à assinatura.
        /// </summary>
        public virtual Seller Seller { get; set; }

        /// <summary>
        /// Status da assinatura.
        /// </summary>
        public SubscriptionStatus Status { get; set; }

        //public SubscriptionMode Mode { get; set; }

        /// <summary>
        /// Plano da assinatura.
        /// </summary>
        public Plan Plan { get; set; }

        /// <summary>
        /// Data de início da assinatura.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Data de encerramento da assinatura.
        /// </summary>
        public DateTime? FinishDate { get; set; }

        /// <summary>
        /// Valor da assinatura.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Sigla da moeda do valor da assinatura.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Identificador da assinatura no gateway de pagamento.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Url de postback, onde o gateway de pagamento envia os dados da assinatura quando há transação de pagamento/cancelamento.
        /// </summary>
        public string PostbackUrl { get; set; }

        public DateTime? LastCreditsAddDate { get; set; }

        public DateTime? NextCreditsAddDate { get; set; }

        public ICustomer Customer => Seller;

        IPlan ISubscription.Plan => Plan;

        public ITransaction CurrentTransaction => null;

        public string GatewayId => ReferenceId;

        /// <summary>
        /// Indica se a assinatura é válida
        /// </summary>
        public bool IsValid =>
            Status == SubscriptionStatus.Trialing ||
            Status == SubscriptionStatus.Paid ||
            Status == SubscriptionStatus.PendingPayment;

        /// <summary>
        /// Indica se a assinatura está ativa.
        /// </summary>
        public bool IsActive =>
            Status == SubscriptionStatus.Trialing ||
            Status == SubscriptionStatus.Paid;

        public void SetCurrentStatus(SubscriptionStatus status)
        {
            Status = status;
        }

        /// <summary>
        /// Transações de pagamento/cancelamento realizadas na assinatura.
        /// </summary>
        public IList<SubscriptionTransaction> Transactions { get; set; }

        public Subscription()
        {
            StartDate = DateTime.UtcNow;
            Transactions = new List<SubscriptionTransaction>();
        }

        /// <summary>
        /// Adiciona uma transação à assinatura.
        /// </summary>
        /// <param name="referenceId">Identificador da transação no gateway de pagamento.</param>
        /// <param name="status">Status da transação.</param>
        /// <param name="content">Conteúdo da transação, vindo do gateway de pagamento.</param>
        /// <param name="subsReferenceId">Identificador da assinatura no gateway de pagamento, relacionada à transação.</param>
        public void AddTransaction(string referenceId, string status, string content, string subsReferenceId)
        {
            Transactions.Add(new SubscriptionTransaction
            {
                ReferenceId = referenceId,
                Status = status,
                Content = content,
                SubsReferenceId = subsReferenceId
            });
        }

        /// <summary>
        /// Cancela a assinatura.
        /// </summary>
        public void Cancel()
        {
            Status = SubscriptionStatus.Canceled;
            FinishDate = DateTime.UtcNow;
            NextCreditsAddDate = null;
            AddDomainEvent(new SubscriptionCancelledEvent(this));
            AddDomainEvent(new SellerProfileUpdated(this.SellerId));
        }

        public object GetId()
        {
            return SellerId;
        }

        #region Domain Events

        private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();

        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        #endregion
    }
}