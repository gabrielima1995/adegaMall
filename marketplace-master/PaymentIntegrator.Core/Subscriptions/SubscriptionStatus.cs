using System.ComponentModel;

namespace PaymentIntegrator.Core.Subscriptions
{
    /// <summary>
    /// Status de assinatura.
    /// </summary>
    public enum SubscriptionStatus
    {
        /// <summary>
        /// N/A.
        /// </summary>
        [Description("SUBSCRIPTION_NONE_TEXT")]
        None = 0,

        /// <summary>
        /// Assinatura em período trial (teste).
        /// </summary>
        [Description("SUBSCRIPTION_TRIAL_TEXT")]
        Trialing = 1,

        /// <summary>
        /// Assinatura paga.
        /// </summary>
        [Description("SUBSCRIPTION_PAID_TEXT")]
        Paid = 2,

        /// <summary>
        /// Aguardando pagamento da assinatura.
        /// </summary>
        [Description("SUBSCRIPTION_PENDINGPAYMENT_TEXT")]
        PendingPayment = 3,

        /// <summary>
        /// Assinatura não paga.
        /// </summary>
        [Description("SUBSCRIPTION_UNPAID_TEXT")]
        Unpaid = 4,

        /// <summary>
        /// Assinatura cancelada.
        /// </summary>
        [Description("SUBSCRIPTION_CANCELED_TEXT")]
        Canceled = 5,

        /// <summary>
        /// Assinatura finalizada.
        /// </summary>
        [Description("SUBSCRIPTION_ENDED_TEXT")]
        Ended = 6,

        /// <summary>
        /// Assinatura recusada.
        /// </summary>
        [Description("SUBSCRIPTION_REFUSED_TEXT")]
        Refused = 99
    }

    public static class SubscriptionStatusExtensions
    {
        public static bool IsActiveSubscription(this SubscriptionStatus status) =>
            status == SubscriptionStatus.Trialing ||
            status == SubscriptionStatus.Paid ||
            status == SubscriptionStatus.PendingPayment;
    }
}