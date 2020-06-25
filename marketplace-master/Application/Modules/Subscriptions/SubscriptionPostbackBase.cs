using Domain.Entities;
using Domain.Entities.SubscriptionModule;
using Domain.Events.SellerProfileUpdated;
using Domain.Events.Subscriptions;
using PaymentIntegrator.Core.Subscriptions;
using System;

namespace Application.Modules.Subscriptions
{
    /// <summary>
    /// Representa os dados de alteração de assinatura, enviados via postback pelo gateway de pagamento.
    /// O postback é enviado toda a vez que o status da assinatura é alterado no gateway, seja por pagamento, cancelamento ou expiração.
    /// </summary>
    public abstract class SubscriptionPostbackBase
    {
        /// <summary>
        /// Status atual da assinatura.
        /// </summary>
        public abstract SubscriptionStatus Status { get; }

        /// <summary>
        /// Indica se é uma transação de cancelamento de assinatura.
        /// </summary>
        public bool IsCancelTransaction =>
            Status == SubscriptionStatus.Canceled ||
            Status == SubscriptionStatus.Unpaid;

        /// <summary>
        /// Indica se é uma transação de pagamento de assinatura.
        /// </summary>
        public bool IsPaymentTransaction =>
            Status == SubscriptionStatus.Paid ||
            Status == SubscriptionStatus.PendingPayment ||
            Status == SubscriptionStatus.Trialing;

        /// <summary>
        /// Indica se é uma transação de falha de pagamento de assinatura.
        /// </summary>
        public bool IsFailedTransaction =>
            Status == SubscriptionStatus.Refused;

        /// <summary>
        /// Indica se é uma transação de assinatura expirada.
        /// </summary>
        public bool IsExpiredTransaction =>
            Status == SubscriptionStatus.Ended;

        /// <summary>
        /// Valor pago na transação.
        /// </summary>
        public abstract decimal TransactionAmount { get; }

        /// <summary>
        /// Data de assinatura.
        /// </summary>
        public abstract DateTime SubscriptionDate { get; }

        /// <summary>
        /// Identificador da assinatura no gateway de pagamento.
        /// </summary>
        public abstract string GatewayId { get; }

        /// <summary>
        /// Moeda usada no pagamento da transação.
        /// </summary>
        public abstract string Currency { get; }

        /// <summary>
        /// Url de postback, onde o gateway de pagamento envia os dados da assinatura quando há transação de pagamento/cancelamento.
        /// </summary>
        public abstract string PostbackUrl { get; }

        /// <summary>
        /// Identificador da transação no gateway de pagamento.
        /// </summary>
        public abstract string TransactionId { get; }

        /// <summary>
        /// Status da transação no gateway de pagamento.
        /// </summary>
        public abstract string TransactionStatus { get; }

        /// <summary>
        /// Atualiza a assinatura com base nos dados do postback.
        /// </summary>
        /// <param name="subscription">Dados da assinatura</param>
        /// <param name="plan">Dados do plano de assinatura.</param>
        public void UpdateSubscription(Subscription subscription, Plan plan)
        {
            subscription.Status = Status;

            subscription.FinishDate = IsCancelTransaction || IsFailedTransaction
                ? (DateTime?)DateTime.UtcNow
                : null;

            subscription.ReferenceId = GatewayId;
            subscription.PostbackUrl = PostbackUrl;

            if (IsPaymentTransaction)
            {
                subscription.Amount = TransactionAmount;
                subscription.Currency = Currency;
            }

            subscription.Plan = plan;

            subscription.AddTransaction(TransactionId, TransactionStatus, this.ToJson(), subscription.ReferenceId);

            subscription.Seller.NotAllowedTrial = true;
            subscription.AddDomainEvent(new SellerProfileUpdated(subscription.Seller.Id));
            subscription.AddDomainEvent(new SubscriptionUpdatedEvent(subscription.Seller.Id, subscription.Plan.Id, subscription.Plan.Name));
        }

        /// <summary>
        /// Cria uma nova assinatura com base nos dados de postback.
        /// </summary>
        /// <param name="seller">Dados do assinante.</param>
        /// <param name="plan">Dados do plano de assinatura.</param>
        /// <returns></returns>
        public Subscription CreateSubscription(Seller seller, Plan plan)
        {
            var subscription = new Subscription { Seller = seller };

            UpdateSubscription(subscription, plan);

            subscription.AddDomainEvent(new SubscriptionAddedEvent(seller.Id, plan.Id, plan.Name));
            subscription.AddDomainEvent(new SellerProfileUpdated(seller.Id));
            return subscription;
        }
    }
}