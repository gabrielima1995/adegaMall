namespace Domain.Entities.StoreModule
{
    /// <summary>
    /// Enumera os possíveis tipos de transação de estoque de produtos.
    /// </summary>
    public enum StockTransactionType
    {
        /// <summary>
        /// Estoque inicial, quando o produto é cadastrado.
        /// </summary>
        StockOpening = 0,

        /// <summary>
        /// Quando o produto é vendido, resultando em baixa no estoque.
        /// </summary>
        NewSale = 1,

        /// <summary>
        /// Quando uma venda é cancelada por qualquer motivo, resultando em retorno do produto ao estoque.
        /// </summary>
        SaleCancelledOrRejected = 2,

        /// <summary>
        /// Quando o estoque do produto é atualizado manualmente.
        /// </summary>
        ManualUpdate = 3
    }
}