using Framework;
using PaymentIntegrator.Core.Customers;

namespace PaymentIntegrator.Core.PaymentMethods
{
    /// <summary>
    /// Operações de métodos de pagamento no gateway de pagamento.
    /// </summary>
    public interface IPaymentMethodOperations
    {
        /// <summary>
        /// Cadastra um cartão de crédito no gateway de pagamento.
        /// </summary>
        /// <param name="customer">Dados do comprador/cliente.</param>
        /// <param name="creditCard">Dados do cartão de crédito.</param>
        /// <returns></returns>
        OperationResult<ICreditCardResult> CreateCreditCard(ICustomer customer, ICreditCard creditCard, bool isSubscribe = false);
    }
}