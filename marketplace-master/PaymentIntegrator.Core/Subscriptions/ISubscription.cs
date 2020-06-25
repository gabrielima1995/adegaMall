using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Core.Plans;

namespace PaymentIntegrator.Core.Subscriptions
{
    /// <summary>
    /// Representa um assinatura.
    /// </summary>
    public interface ISubscription
    {
        /// <summary>
        /// Status da assinatura.
        /// </summary>
        SubscriptionStatus Status { get; }

        /// <summary>
        /// Cliente.
        /// </summary>
        ICustomer Customer { get; }

        /// <summary>
        /// Plano da assinatura.
        /// </summary>
        IPlan Plan { get; }

        /// <summary>
        /// Última transação realizada na assinatura.
        /// </summary>
        ITransaction CurrentTransaction { get; }

        /// <summary>
        /// Endpoint da aplicação para receber as mudanças de status da assinatura.
        /// </summary>
        string PostbackUrl { get; }

        /// <summary>
        /// Identificador da assinatura no gateway de pagamento.
        /// </summary>
        string GatewayId { get; }

        /// <summary>
        /// Atribui novo status para a assinatura.
        /// </summary>
        /// <param name="status">Novo status da assinatura.</param>
        void SetCurrentStatus(SubscriptionStatus status);
    }
}