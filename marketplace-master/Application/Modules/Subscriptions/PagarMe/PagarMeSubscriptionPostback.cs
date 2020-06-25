using PaymentIntegrator.Core.Subscriptions;
using System;

namespace Application.Modules.Subscriptions.PagarMe
{
    /// <summary>
    /// Representa os dados de alteração de assinatura, enviados via postback pelo Pagar.me.
    /// </summary>
    public class PagarMeSubscriptionPostback : SubscriptionPostbackBase
    {
        #region Constants

        private const string trialing = "trialing";

        private const string paid = "paid";

        private const string pending_payment = "pending_payment";

        private const string unpaid = "unpaid";

        private const string canceled = "canceled";

        private const string ended = "ended";

        private const string refused = "refused";

        #endregion Constants

        public string id { get; set; }

        public string fingerprint { get; set; }

        public string @event { get; set; }

        public string old_status { get; set; }

        public string desired_status { get; set; }

        public string current_status { get; set; }

        public SubscriptionPostbackData subscription { get; set; }

        public override SubscriptionStatus Status
        {
            get
            {
                if (current_status == canceled) // cancelado pelo usuário
                    return SubscriptionStatus.Canceled;

                if (current_status == unpaid) // após tentativas de cobrança, o assinante não pagou a assinatura
                    return SubscriptionStatus.Unpaid;

                if (current_status == trialing) // período de trial
                    return SubscriptionStatus.Trialing;

                if (current_status == paid) // usuário pagou
                    return SubscriptionStatus.Paid;

                if (current_status == pending_payment) // aguardando pagamento
                    return SubscriptionStatus.PendingPayment;

                if (current_status == refused) // pagamento recusado
                    return SubscriptionStatus.Refused;

                if (current_status == ended) // recorrencia finalizada
                    return SubscriptionStatus.Ended;

                return SubscriptionStatus.None;// sem status
            }
        }

        /// <summary>
        /// Valor pago na transação.
        /// </summary>
        public override decimal TransactionAmount
            => (subscription.current_transaction?.amount ?? subscription.plan.amount) / 100M;

        /// <summary>
        /// Data de assinatura.
        /// </summary>
        public override DateTime SubscriptionDate
            => subscription.date_created;

        /// <summary>
        /// Identificador da assinatura no gateway de pagamento.
        /// </summary>
        public override string GatewayId
            => id;

        /// <summary>
        /// Moeda usada no pagamento da transação.
        /// </summary>
        public override string Currency =>
            "BRL";

        public override string PostbackUrl =>
            subscription.postback_url;

        /// <summary>
        /// Identificador da transação no gateway de pagamento.
        /// </summary>
        public override string TransactionId =>
            subscription.current_transaction?.id;

        /// <summary>
        /// Status da transação no gateway de pagamento.
        /// </summary>
        public override string TransactionStatus =>
            subscription.current_transaction?.status ?? current_status;

        public PagarMeSubscriptionPostback()
        {
        }

        public PagarMeSubscriptionPostback(ISubscription subscription)
        {
            id = subscription.GatewayId;
            current_status = GetStatusString(subscription.Status); // signup
            this.subscription = new SubscriptionPostbackData(subscription);
        }

        private string GetStatusString(SubscriptionStatus status)
        {
            if (status == SubscriptionStatus.Canceled)
                return canceled;

            if (status == SubscriptionStatus.Unpaid)
                return unpaid;

            if (status == SubscriptionStatus.Trialing)
                return trialing;

            if (status == SubscriptionStatus.Paid)
                return paid;

            if (status == SubscriptionStatus.PendingPayment)
                return pending_payment;

            if (status == SubscriptionStatus.Refused)
                return refused;

            if (status == SubscriptionStatus.Ended)
                return ended;

            return "";
        }
    }
}