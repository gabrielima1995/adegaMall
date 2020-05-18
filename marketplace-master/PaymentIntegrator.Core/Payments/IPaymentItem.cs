namespace PaymentIntegrator.Core.Payments
{
    /// <summary>
    /// Representa um item para pagamento no gateway de pagamento.
    /// </summary>
    public interface IPaymentItem
    {
        /// <summary>
        /// Descrição do item.
        /// </summary>
        string Item { get; }

        /// <summary>
        /// Quantidade do item.
        /// </summary>
        decimal Quantity { get; }

        /// <summary>
        /// Preço unitário do item.
        /// </summary>
        decimal Price { get; }

        /// <summary>
        /// Preço total do item.
        /// </summary>
        decimal Amount { get; }

        /// <summary>
        /// Preço total do item, sem frete e sem taxas de parcelamento
        /// </summary>
        decimal SubTotal { get; }

        bool PriceRequired { get; }
    }
}