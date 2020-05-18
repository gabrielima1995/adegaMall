using Framework;
using PaymentIntegrator.Core.Common;

namespace PaymentIntegrator.Core.Customers
{
    /// <summary>
    /// Operações de comprador/cliente no gateway de pagamento.
    /// </summary>
    public interface ICustomerOperations
    {
        /// <summary>
        /// Cadastra um comprador/cliente no gateway de pagamento.
        /// </summary>
        /// <param name="customer">Dados do comprador/cliente.</param>
        /// <param name="address">Endereço do comprador/cliente.</param>
        /// <returns></returns>
        OperationResult Create(ICustomer customer, IAddress address);

        /// <summary>
        /// Atualiza os dados de comprador/cliente no gateway de pagamento.
        /// </summary>
        /// <param name="customer">Dados do comprador/cliente.</param>
        /// <param name="address">Endereço do comprador/cliente.</param>
        /// <returns></returns>
        OperationResult Update(ICustomer customer, IAddress address);
    }
}