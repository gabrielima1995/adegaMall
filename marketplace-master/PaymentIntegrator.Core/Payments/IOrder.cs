using PaymentIntegrator.Core.Suppliers;
using System.Collections.Generic;

namespace PaymentIntegrator.Core.Payments
{
    /// <summary>
    /// Representa um pedido de venda.
    /// </summary>
    public interface IOrder
    {
        /// <summary>
        /// Identificador do pedido.
        /// </summary>
        string OrderId { get; }

        /// <summary>
        /// Itens do pedido.
        /// </summary>
        IList<IPaymentItem> Items { get; }

        /// <summary>
        /// Dados do fornecedor (caso for marketplace).
        /// </summary>
        ISupplier Supplier { get; }

        /// <summary>
        /// Desconto no pedido.
        /// </summary>
        decimal Discount { get; }

        /// <summary>
        /// Valor do frete no pedido.
        /// </summary>
        decimal Shipping { get; }

        /// <summary>
        /// Total líquido do pedido.
        /// </summary>
        decimal Amount { get; }

        /// <summary>
        /// Valor da taxa da startup (caso for marketplace).
        /// </summary>
        decimal StartupFee { get; }

        /// <summary>
        /// Taxas e juros de parcelamento a serem repassados
        /// </summary>
        decimal InstallmentFees { get; }

        /// <summary>
        /// Indica se o fornecedor é responsável pelas taxas da transação (caso for marketplace).
        /// </summary>
        bool ProcessingGatewayFee { get; }

        /// <summary>
        /// Identificador da regra de split (caso for marketplace).
        /// </summary>
        string SplitId { get; }

        /// <summary>
        /// Atribui o valor da taxa da startup, caso for marketplace.
        /// </summary>
        /// <param name="startupFee">Valor da taxa para a startup.</param>
        void SetStartupFee(decimal startupFee);

        /// <summary>
        /// Atribui o identificador da regra de split, caso for marketplace.
        /// </summary>
        /// <param name="splitId">Identificador da regra de split.</param>
        void SetSplitId(string splitId);

        /// <summary>
        /// Indica se o fornecedor é responsável pelas taxas da transação (caso for marketplace).
        /// </summary>
        /// <param name="processingGatewayFee">Verdadeiro se o fornecedor é responsável pelas taxas da transação</param>
        void SetProcessingGatewayFee(bool processingGatewayFee);

        /// <summary>
        /// Calcula o valor das taxas que são retidas pela marketplace
        /// </summary>
        /// <returns>Total de taxas retido pela marketplace</returns>
        decimal GetSplitFees();

        /// <summary>
        /// Determina se o recebedor também é responsável pelo chargeback
        /// </summary>
        /// <returns></returns>
        bool IsLiable();
    }
}