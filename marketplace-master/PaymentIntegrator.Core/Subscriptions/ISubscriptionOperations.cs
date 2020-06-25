using Framework;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.PaymentMethods;
using PaymentIntegrator.Core.Plans;

namespace PaymentIntegrator.Core.Subscriptions
{
    /// <summary>
    /// Operações de assinatura no gateway de pagamento.
    /// </summary>
    public interface ISubscriptionOperations
    {
        /// <summary>
        /// Assina, para um cliente, um plano de assinatura, com pagamento via cartão de crédito, no gateway de pagamento.
        /// </summary>
        /// <param name="plan">Plano de assinatura.</param>
        /// <param name="creditCard">Dados do cartão de crédito.</param>
        /// <param name="customer">Dados do cliente.</param>
        /// <param name="address">Endereço do cliente.</param>
        /// <param name="postbackUrl">Endpoint da aplicação para receber as mudanças de status da assinatura.</param>
        /// <returns></returns>
        OperationResult<ISubscription> Subscribe(IPlan plan, ICreditCard creditCard,
            ICustomer customer, IAddress address, string postbackUrl);

        /// <summary>
        /// Assina, para um cliente, um plano de assinatura, com pagamento via cartão de crédito, no gateway de pagamento.
        /// </summary>
        /// <param name="plan">Plano de assinatura.</param>
        /// <param name="creditCardStatus">Dados do cartão de crédito, já criado no gateway de pagamento.</param>
        /// <param name="customer">Dados do cliente.</param>
        /// <param name="address">Endereço do cliente.</param>
        /// <param name="postbackUrl">Endpoint da aplicação para receber as mudanças de status da assinatura.</param>
        /// <returns></returns>
        OperationResult<ISubscription> Subscribe(IPlan plan, ICreditCardResult creditCardStatus,
            ICustomer customer, IAddress address, string postbackUrl);

        /// <summary>
        /// Cancela um assinatura no gateway de pagamento.
        /// </summary>
        /// <param name="subscription">Dados da assinatura.</param>
        /// <returns></returns>
        OperationResult<ISubscription> Unsubscribe(ISubscription subscription);

        /// <summary>
        /// Verifica o status de uma assinatura no gateway
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        OperationResult<SubscriptionStatus> GetSubscriptionStatus(ISubscription subscription);
    }
}