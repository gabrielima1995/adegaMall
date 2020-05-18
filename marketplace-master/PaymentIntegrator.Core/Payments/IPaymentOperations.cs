using Framework;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.Suppliers;
using System.Collections.Generic;

namespace PaymentIntegrator.Core.Payments
{
    /// <summary>
    /// Operações de pagamento no gateway de pagamento.
    /// </summary>
    public interface IPaymentOperations
    {
        /// <summary>
        /// Recupera os dados de uma transação.
        /// </summary>
        /// <param name="transactionId">O id da transação no Gateway.</param>
        /// <returns>Dados da transação.</returns>
        OperationResult<ITransaction> GetTransaction(string transactionId);

        /// <summary>
        /// Realiza pagamento com cartão de crédito para um item.
        /// </summary>
        /// <param name="paymentItem">Item para pagamento.</param>
        /// <param name="customer">Dados do cliente que está comprando o item.</param>
        /// <param name="paymentOptions">Opções de pagamento.</param>
        /// <param name="supplier">Dados do fornecedor do item, no caso de marketplace.</param>
        /// <param name="buyerAddress">Endereço do comprador, no caso de anti-fraude habilitado.</param>
        /// <returns>Dados da transação.</returns>
        OperationResult<ITransaction> Pay(IPaymentItem paymentItem, ICustomer customer, IPaymentOptions paymentOptions, ISupplier supplier = null, IAddress buyerAddress = null);

        /// <summary>
        /// Realiza pagamento com cartão de crédito para uma lista de itens.
        /// </summary>
        /// <param name="paymentItems">Itens para pagamento.</param>
        /// <param name="customer">Dados do cliente que está comprando o item.</param>
        /// <param name="paymentOptions">Opções de pagamento.</param>
        /// <param name="supplier">Dados do fornecedor do item, no caso de marketplace.</param>
        /// <param name="buyerAddress">Endereço do comprador, no caso de anti-fraude habilitado.</param>
        /// <returns>Dados da transação.</returns>
        OperationResult<ITransaction> Pay(IList<IPaymentItem> paymentItems, ICustomer customer, IPaymentOptions paymentOptions, ISupplier supplier = null, IAddress buyerAddress = null);

        /// <summary>
        /// Realiza pagamento com cartão de crédito para um pedido de venda.
        /// </summary>
        /// <param name="order">Dados do pedido de venda.</param>
        /// <param name="customer">Dados do cliente que está fez o pedido.</param>
        /// <param name="paymentOptions">Opções de pagamento.</param>
        /// <param name="buyerAddress">Endereço do comprador, no caso de anti-fraude habilitado.</param>
        /// <returns>Dados da transação.</returns>
        OperationResult<ITransaction> Pay(IOrder order, ICustomer customer, IPaymentOptions paymentOptions, IAddress buyerAddress = null);

        /// <summary>
        /// Realiza pagamento com cartão de crédito para uma lista de pedidos de venda.
        /// </summary>
        /// <param name="orders">Lista de pedidos de venda.</param>
        /// <param name="customer">Dados do cliente que está fez o pedido.</param>
        /// <param name="paymentOptions">Opções de pagamento.</param>
        /// <param name="buyerAddress">Endereço do comprador.</param>
        /// <returns>Dados da transação.</returns>
        OperationResult<ITransaction> Pay(IList<IOrder> orders, ICustomer customer, IPaymentOptions paymentOptions, IAddress buyerAddress = null);

        /// <summary>
        /// Estorna o valor de um pedido de venda para determinada transação.
        /// </summary>
        /// <param name="order">Dados do pedido de venda.</param>
        /// <param name="transaction">Dados da transação.</param>
        /// <param name="totalRefund">Valor total do estorno.</param>
        /// <returns>Dados da transação.</returns>
        OperationResult<ITransaction> Refund(IOrder order, ITransaction transaction, decimal totalRefund);

        /// <summary>
        /// Retorna os detalhes de um recebível de uma parcela de uma transação, 
        /// O recebedor deve ser especificado neste caso.
        /// </summary>
        /// <returns></returns>
        OperationResult<PayableDto> GetPayableDetails(string transactionId, string recipientId, int installment);

        /// <summary>
        /// Retorna os detalhes de um recebível por ID no Gateway
        /// </summary>
        /// <returns></returns>
        OperationResult<PayableDto> GetPayableDetails(string payableId);

        /// <summary>
        /// Retorna os detalhes de uma transferência por ID no Gateway
        /// </summary>
        /// <returns></returns>
        OperationResult<TransferDto> GetTransferDetails(string transferId);

        /// <summary>
        /// Gera uma transferência no gateway relativa a um recebível
        /// </summary>
        /// <param name="payableId">ID do recebível no gateway</param>
        /// <param name="recipientId">ID do recebedor (destinatário da transferência)</param>
        /// <param name="amount">O valor da transferência</param>
        OperationResult<TransferDto> CreateTransferForPayable(string payableId);

        /// <summary>
        /// Lista as transferências para conta bancária (saques/retiradas) de um vendedor.
        /// </summary>
        /// <param name="supplierId">O id do vendedor no gateway</param>
        /// <returns>A lista das transferências do vendedor</returns>
        OperationResult<IReadOnlyCollection<TransferDto>> ListWithdraws(string supplierId, string supplierApiToken);
    }
}